using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types;
using Telegram.Bot;
using WatchUILibrary;
using WatchUILibrary.Models;

namespace WatchDb.TelegramAPI.Controllers
{
    [ApiController]
    [Route("api/tg/admin")]
    public class TgAdminController
    {
        private TelegramBotClient admin;

        private ShopDbContext context;
        private Dictionary<ReplyCodes, Func<Update, Task>> replyCodesHandles = new Dictionary<ReplyCodes, Func<Update, Task>>();
        private Dictionary<string, Func<Update, Task>> commandHandles = new Dictionary<string, Func<Update, Task>>();


        private static Producer? producer;
        private static Category? category;
        private static Watch? newWatch;
        private static ChatId chat;


        public TgAdminController(IConfiguration configuration, ShopDbContext context)
        {
            this.admin = new TelegramBotClient(configuration["TgTokens:Admin"]);

            this.context = context;
            replyCodesHandles.Add(ReplyCodes.GetCategories, SendCategoriesKeyboardAsync);
            replyCodesHandles.Add(ReplyCodes.GetProducers, SendProducersKeyboardAsync);
            replyCodesHandles.Add(ReplyCodes.CreateNewCategory, SendCreateNewCategoryMessageAsync);
            replyCodesHandles.Add(ReplyCodes.CreateNewProducer, SendCreateNewProducerMessageAsync);
            replyCodesHandles.Add(ReplyCodes.SetTitle, SendCreateNewTitleMessageAsync);
            replyCodesHandles.Add(ReplyCodes.SetPrice, SendCreateNewPriceMessageAsync);
            replyCodesHandles.Add(ReplyCodes.CreateNewWatch, CreateWatchAsync);
            replyCodesHandles.Add(ReplyCodes.GetOrders, SendOrdersAsync);
            replyCodesHandles.Add(ReplyCodes.GetUsers, SendUsersAsync);
            replyCodesHandles.Add(ReplyCodes.GetWatches, SendWatchesAsync);
            replyCodesHandles.Add(ReplyCodes.GetAddWatchMenu, CreateNewWatchAsync);
            commandHandles.Add("/category", CreateCategoryAsync);
            commandHandles.Add("/all", SendWatchesAsync);
            commandHandles.Add("/producer", CreateProducerAsync);
            commandHandles.Add("/model", CreateTitleAsync);
            commandHandles.Add("/price", CreatePriceAsync);
            commandHandles.Add("/orders", SendOrdersAsync);
            commandHandles.Add("/users", SendUsersAsync);
            commandHandles.Add("/new", CreateNewWatchAsync);
            commandHandles.Add("/message", SendMessageToUserAsync);
            commandHandles.Add("/writeall", SendMessageToAllUsersAsync);
        }


        [HttpPost]
        public async Task<IResult> Post(Update update)
        {
            if (update != null)
            {
                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message != null && update.Message.Text != null)
                {
                    await HandleCommandAsync(update);
                }
                else if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && update.CallbackQuery != null && update.CallbackQuery.Data != null && update.CallbackQuery.From != null)
                {
                    await HandleCallbackQueryAsync(update);
                }
            }
            return Results.Ok();
        }

        private async Task HandleCommandAsync(Update update)
        {
            string command = TgHelper.GetCommand(update.Message!.Text!);

            if (commandHandles.ContainsKey(command))
            {
                await commandHandles[command](update);
            }

            else
            {
                await SendAdminKeyboardAsync(update);
            }
        }

        private async Task HandleCallbackQueryAsync(Update update)
        {
            if (Enum.TryParse<ReplyCodes>(update.CallbackQuery!.Data, out ReplyCodes code))
            {
                if (replyCodesHandles.ContainsKey(code))
                {
                    await replyCodesHandles[code](update);
                }
            }
            else if (update.CallbackQuery.Data!.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.CategoryId)!))
            {
                if (TgHelper.GetIntFromCallbackQuery(update, ReplyCodes.CategoryId, out int id))
                {
                    category = await context.Categories.GetAsync(id);
                }
                await SendAddNewWatchKeyboardAsync(update);
            }
            else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.ProducerId)!))
            {
                if (TgHelper.GetIntFromCallbackQuery(update, ReplyCodes.ProducerId, out int id))
                {
                    producer = await context.Producers.GetAsync(id);
                }
                await SendAddNewWatchKeyboardAsync(update);
            }
            else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.DeleteWatch)!))
            {
                if (TgHelper.GetIntFromCallbackQuery(update, ReplyCodes.DeleteWatch, out int id))
                {
                    await context.Watches.DeleteAsync(id);
                    await admin.SendTextMessageAsync(update.CallbackQuery.From.Id, "Deleted");
                }
                else
                {
                    await admin.SendTextMessageAsync(update.CallbackQuery.From.Id, "Command is not recognized");
                }
                await SendAdminKeyboardAsync(update);
            }
            else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.CloseOrder)!))
            {
                if (TgHelper.GetIntFromCallbackQuery(update, ReplyCodes.CloseOrder, out int id))
                {
                    await context.Orders.CloseOrderAsync(id);
                    await admin.SendTextMessageAsync(update.CallbackQuery.From.Id, "Closed");
                }
                else
                {
                    await admin.SendTextMessageAsync(update.CallbackQuery.From.Id, "Command is not recognized");
                }
                await SendAdminKeyboardAsync(update);
            }

            else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.WriteToUser)!))
            {
                if (long.TryParse(update.CallbackQuery.Data
                    .Substring(Enum.GetName<ReplyCodes>(ReplyCodes.WriteToUser)!.Length).Trim(), out long id))
                {
                    chat = id;
                    await admin.SendTextMessageAsync(update.CallbackQuery.From.Id, "Write /message [message]");
                }
                else
                {
                    await admin.SendTextMessageAsync(update.CallbackQuery.From.Id, "Command is not recognized");
                    await SendAdminKeyboardAsync(update);
                }
            }
        }

        // Keyboards

        private async Task SendAdminKeyboardAsync(Update update)
        {
            await admin.SendAdminKeyboardAsync(update);
        }

        private async Task SendAddNewWatchKeyboardAsync(Update update)
        {
            if (newWatch != null)
            {
                await admin.SendAddNewWatchKeyboardAsync(update, category, producer, newWatch);
            }
        }

        private async Task SendCategoriesKeyboardAsync(Update update)
        {
             await admin.SendCategoriesKeyboardAsync(update, await context.Categories.GetAsync(), true);
        }

        private async Task SendProducersKeyboardAsync(Update update)
        {
            await admin.SendProducersKeyboardAsync(update, await context.Producers.GetAsync(), true);
        }


        // ===========================================


        // Creating
        private async Task CreateNewWatchAsync(Update update)
        {
            newWatch = new Watch();

            await SendAddNewWatchKeyboardAsync(update);
        }

        private async Task CreateProducerAsync(Update update)
        {
            var name = TgHelper.GetValueFromCommand(update, "/producer");
            await context.Producers.CreateAsync(new Producer()
            {
                ProducerName = name
            });

            await SendAddNewWatchKeyboardAsync(update);
        }

        private async Task CreateCategoryAsync(Update update)
        {
            var name = TgHelper.GetValueFromCommand(update, "/category");
            await context.Categories.CreateAsync(new Category()
            {
                CategoryName = name
            });

            await SendAddNewWatchKeyboardAsync(update);
        }

        private async Task CreateTitleAsync(Update update)
        {
            var name = TgHelper.GetValueFromCommand(update, "/model");
            if (newWatch != null)
            {
                newWatch.Title = name;
            }

            await SendAddNewWatchKeyboardAsync(update);
        }

        private async Task CreatePriceAsync(Update update)
        {
            if (update != null && update.Message != null && update.Message.Text != null)
            {
                if (decimal.TryParse(update.Message.Text.Substring("/price".Length).Trim(), out decimal price))
                {
                    if (newWatch != null)
                    {
                        newWatch.Price = price;
                    }
                }
            }

            await SendAddNewWatchKeyboardAsync(update);
        }

        private async Task CreateWatchAsync(Update update)
        {
            if (newWatch != null)
            {
                newWatch.Producer = producer;
                newWatch.Category = category;
                await context.Watches.CreateAsync(newWatch);
                await admin.SendResponseAsync(update, "Created");
            }
        }

        // ============================================



        // Messages

        private async Task SendMessageToUserAsync(Update update)
        {
            if (update != null && update.Message != null && update.Message.Text != null)
            {
                await admin.SendTextMessageAsync(chat, update.Message.Text);
            }
        }

        private async Task SendMessageToAllUsersAsync(Update update)
        {
            if (update != null && update.Message != null && update.Message.Text != null)
            {
                var msg = TgHelper.GetValueFromCommand(update, "/writeall");
                if (string.IsNullOrWhiteSpace(msg))
                {
                    return;
                }

                (await context.Users.GetAsync()).ForEach(async u => await admin.SendTextMessageAsync(u.ChatId, update.Message.Text));
            }
        }

        private async Task SendCreateNewTitleMessageAsync(Update update)
        {
            await admin.SendResponseAsync(update, "Write /model [value]");
        }

        private async Task SendCreateNewPriceMessageAsync(Update update)
        {
            await admin.SendResponseAsync(update, "Write /price [value]");
        }

        private async Task SendCreateNewCategoryMessageAsync(Update update)
        {
            await admin.SendResponseAsync(update, "Write /category [name]");
        }

        private async Task SendCreateNewProducerMessageAsync(Update update)
        {
            await admin.SendResponseAsync(update, "Write /producer [name]");
        }

        // ============================================


        // Data

        private async Task SendWatchesAsync(Update update)
        {
            var watches = await context.Watches.GetAsync();

            await admin.SendWatchesAsync(update, watches, true);
        }

        private async Task SendUsersAsync(Update update)
        {
            var users = await context.Users.GetAsync();

            await admin.SendUsersAsync(update, users);
        }

        private async Task SendOrdersAsync(Update update)
        {
            var orders = await context.Orders.GetAsync();

            await admin.SendOrdersAsync(update, orders);
        }


        // ============================================
    }
}
