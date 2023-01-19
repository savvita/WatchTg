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
    [Route("api/tg")]
    public class TgUserController
    {
        private TelegramBotClient bot;

        private ShopDbContext context;
        private Dictionary<ReplyCodes, Func<Update, Task>> replyCodesHandles = new Dictionary<ReplyCodes, Func<Update, Task>>();
        private Dictionary<string, Func<Update, Task>> commandHandles = new Dictionary<string, Func<Update, Task>>();


        public TgUserController(IConfiguration configuration, ShopDbContext context)
        {
            this.bot = new TelegramBotClient(configuration["TgTokens:Bot"]);

            this.context = context;
            replyCodesHandles.Add(ReplyCodes.GetCategories, SendCategoriesKeyboardAsync);
            replyCodesHandles.Add(ReplyCodes.GetProducers, SendProducersKeyboardAsync);
            replyCodesHandles.Add(ReplyCodes.SetTitle, SendSetTitleMessage);

            commandHandles.Add("/filters", SendFiltersKeyboardAsync);
            commandHandles.Add("/all", SendWatchesAsync);
            commandHandles.Add("/model", FilterWatchesAsync);
        }



        [HttpPost]
        public async Task<IResult> Post(Update update)
        {
            if (update != null)
            {
                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message != null && update.Message.Text != null && update.Message.From != null)
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
            await AddUserAsync(update);

            string command = TgHelper.GetCommand(update.Message!.Text!);

            if (commandHandles.ContainsKey(command))
            {
                await commandHandles[command](update);
            }
        }

        private async Task HandleCallbackQueryAsync(Update update)
        {
            await AddUserAsync(update);

            if (Enum.TryParse<ReplyCodes>(update.CallbackQuery.Data, out ReplyCodes code))
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
                    await SendWatchesByCategoryAsync(update, id);
                }
            }
            else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.ProducerId)!))
            {
                if (TgHelper.GetIntFromCallbackQuery(update, ReplyCodes.ProducerId, out int id))
                {
                    await SendWatchesByProducerAsync(update, id);
                }
            }
            else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.Buy)!))
            {
                if (TgHelper.GetIntFromCallbackQuery(update, ReplyCodes.Buy, out int id))
                {
                    await BuyWatchAsync(update.CallbackQuery.From.Id, id);
                }
            }
        }

        private async Task AddUserAsync(Update update)
        {
            ChatId chat;
            string? username = null;
            string? firstName = null;

            if(update != null && update.Message != null && update.Message.From != null)
            {
                chat = update.Message.From.Id;
                username = update.Message.From.Username;
                firstName = update.Message.From.FirstName;
            }
            else if(update != null && update.CallbackQuery != null && update.CallbackQuery.From != null)
            {
                chat = update.CallbackQuery.From.Id;
                username = update.CallbackQuery.From.Username;
                firstName = update.CallbackQuery.From.FirstName;
            }
            else
            {
                throw new ArgumentException();
            }

            var user = await context.Users.GetAsync((long)chat.Identifier!);

            if (user == null)
            {
                await context.Users.CreateAsync(new WatchUILibrary.Models.User()
                {
                    ChatId = (long)chat.Identifier!,
                    Username = username,
                    FirstName = firstName
                });
            }
        }

        // Keyboards
        private async Task SendFiltersKeyboardAsync(Update update)
        {
            await bot.SendFiltersKeyboard(update);
        }

        private async Task SendCategoriesKeyboardAsync(Update update)
        {
             await bot.SendCategoriesKeyboardAsync(update, await context.Categories.GetAsync());
        }

        private async Task SendProducersKeyboardAsync(Update update)
        {
             await bot.SendProducersKeyboardAsync(update, await context.Producers.GetAsync());
        }

        // =================================

        private async Task SendWatchesAsync(Update update)
        {
            var watches = await context.Watches.GetAsync();
            await bot.SendWatchesAsync(update, watches);
        }

        private async Task SendWatchesByCategoryAsync(Update update, int categoryId)
        {
            var watches = (await context.Watches.GetAsync()).Where(x => x.Category != null && x.Category.Id == categoryId);

            await bot.SendWatchesAsync(update, watches);
        }

        private async Task SendWatchesByProducerAsync(Update update, int producerId)
        {
            var watches = (await context.Watches.GetAsync()).Where(x => x.Producer != null && x.Producer.Id == producerId);

            await bot.SendWatchesAsync(update, watches);
        }

        private async Task FilterWatchesAsync(Update update)
        {
            var model = TgHelper.GetValueFromCommand(update, "/model");
            var watches = await context.Watches.GetAsync(model);

            await bot.SendWatchesAsync(update, watches);
        }


        private async Task BuyWatchAsync(ChatId chat, int id)
        {
            var watch = await context.Watches.GetAsync(id);
            if(watch == null)
            {
                await bot.SendTextMessageAsync(chat, $"Cannot find a watch with ID {id}");
                return;
            }

            var user = await context.Users.GetAsync((long)chat.Identifier!);

            if (user == null)
            {
                return;
            }

            var order = new Order()
            {
                Date = DateTime.Now,
                Status = new Status() { Id = 1 },
                UserId = user.Id,
                Details = new List<OrderDetail>()
                {
                    new OrderDetail()
                    {
                        WatchId = id,
                        UnitPrice = watch.Price
                    }
                }
            };

            order = await context.Orders.CreateAsync(order);

            await bot.SendTextMessageAsync(chat, $"Your order ID is {order.Id}");
        }


        private async Task SendSetTitleMessage(Update update)
        {
            await bot.SendResponseAsync(update, "Write /model [value]");
        }


    }
}
