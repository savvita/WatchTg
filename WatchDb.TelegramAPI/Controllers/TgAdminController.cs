using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using WatchUILibrary;
using WatchUILibrary.Models;
using System.Reflection.Metadata;
using System.Text;

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
        private static ChatId chat;


        public TgAdminController(IConfiguration configuration, ShopDbContext context)
        {
            this.admin = new TelegramBotClient(configuration["TgTokens:Admin"]);

            this.context = context;
            replyCodesHandles.Add(ReplyCodes.GetCategories, SendCategoriesKeyboard);
            replyCodesHandles.Add(ReplyCodes.GetProducers, SendProducersKeyboard);
            replyCodesHandles.Add(ReplyCodes.CreateNewCategory, SendCreateNewCategoryMessage);
            replyCodesHandles.Add(ReplyCodes.CreateNewProducer, SendCreateNewProducerMessage);
            replyCodesHandles.Add(ReplyCodes.SetTitle, SendCreateNewTitleMessage);
            replyCodesHandles.Add(ReplyCodes.SetPrice, SendCreateNewPriceMessage);
            replyCodesHandles.Add(ReplyCodes.CreateNewWatch, CreateWatch);
            commandHandles.Add("/category", CreateCategory);
            commandHandles.Add("/all", SendWatches);
            commandHandles.Add("/producer", CreateProducer);
            commandHandles.Add("/model", CreateTitle);
            commandHandles.Add("/price", CreatePrice);
            commandHandles.Add("/orders", SendOrders);
            commandHandles.Add("/users", SendUsers);
            commandHandles.Add("/new", CreateNewWatch);
            commandHandles.Add("/message", SendMessage);
            commandHandles.Add("/writeall", SendMessageToAll);
        }
        [HttpPost]
        public async Task<IResult> Post(Update update)
        {
            if (update != null)
            {
                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message != null && update.Message.Text != null)
                {
                    var idx = update.Message.Text.IndexOf(' ');
                    string command;

                    if (idx > 0)
                    {
                        command = update.Message.Text.Substring(0, idx).ToLower();
                    }
                    else
                    {
                        command = update.Message.Text.ToLower();
                    }

                    if (commandHandles.ContainsKey(command))
                    {
                        await commandHandles[command](update);
                        return Results.Ok();
                    }

                    else
                    {
                        await SendAddNewKeyboard(update);
                    }
                }
                else if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
                {
                    if (update.CallbackQuery != null && update.CallbackQuery.Data != null)
                    {
                        if (Enum.TryParse<ReplyCodes>(update.CallbackQuery.Data, out ReplyCodes code))
                        {
                            if (replyCodesHandles.ContainsKey(code))
                            {
                                await replyCodesHandles[code](update);
                            }
                        }
                        else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.CategoryId)!))
                        {
                            if (int.TryParse(update.CallbackQuery.Data
                                .ToLower()
                                .Substring(Enum.GetName<ReplyCodes>(ReplyCodes.CategoryId)!.Length).Trim(), out int id))
                            {
                                category = await context.Categories.GetAsync(id);
                            }
                            await SendAddNewKeyboard(update);
                        }
                        else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.ProducerId)!))
                        {
                            if (int.TryParse(update.CallbackQuery.Data
                                .ToLower()
                                .Substring(Enum.GetName<ReplyCodes>(ReplyCodes.ProducerId)!.Length).Trim(), out int id))
                            {
                                producer = await context.Producers.GetAsync(id);
                            }
                            await SendAddNewKeyboard(update);
                        }
                        else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.DeleteWatch)!))
                        {
                            if (int.TryParse(update.CallbackQuery.Data
                                .ToLower()
                                .Substring(Enum.GetName<ReplyCodes>(ReplyCodes.DeleteWatch)!.Length).Trim(), out int id))
                            {
                                await context.Watches.DeleteAsync(id);
                            }
                            
                        }
                        else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.CloseOrder)!))
                        {
                            if (int.TryParse(update.CallbackQuery.Data
                                .ToLower()
                                .Substring(Enum.GetName<ReplyCodes>(ReplyCodes.CloseOrder)!.Length).Trim(), out int id))
                            {
                                await context.Orders.CloseOrderAsync(id);
                            }

                        }

                        else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.WriteToUser)!))
                        {
                            if (long.TryParse(update.CallbackQuery.Data
                                .ToLower()
                                .Substring(Enum.GetName<ReplyCodes>(ReplyCodes.WriteToUser)!.Length).Trim(), out long id))
                            {
                                chat = id;
                                await admin.SendTextMessageAsync(update.CallbackQuery.From.Id, "Write /message [message]");
                            }

                        }

                    }
                }

            }
            return Results.Ok();
        }



        private async Task SendWatches(Update update)
        {
            if (update != null)
            {
                if (update.Message != null && update.Message.From != null)
                {
                    var watches = await context.Watches.GetAsync();

                    await admin.SendWatchesAsync(update.Message.From.Id, watches, true);
                }
            }
        }

        private async Task SendUsers(Update update)
        {
            if (update != null)
            {
                if (update.Message != null && update.Message.From != null)
                {
                    var users = await context.Users.GetAsync();

                    if (users.Count() > 0)
                    {

                        foreach (var user in users)
                        {
                            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Write", $"{Enum.GetName<ReplyCodes>(ReplyCodes.WriteToUser)} {user.Id}"));

                            string url = "https://bpgroup.lv/i/product_images/images/Z2000128425.jpg";
                            await admin.SendPhotoAsync(update.Message.From.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(url));
                            await admin.SendTextMessageAsync(update.Message.From.Id, $"{user.ChatId}", replyMarkup: keyboard);
                        }
                    }

                    else
                    {
                        await admin.SendTextMessageAsync(update.Message.From.Id, "Not found");
                    }
                }
            }
        }

        private async Task SendOrders(Update update)
        {
            if (update != null)
            {
                if (update.Message != null && update.Message.From != null)
                {
                    var orders = await context.Orders.GetAsync();

                    if (orders.Count() > 0)
                    {

                        foreach (var order in orders)
                        {
                            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Close", $"{Enum.GetName<ReplyCodes>(ReplyCodes.CloseOrder)} {order.Id}"));

                            StringBuilder sb = new StringBuilder();
                            sb.Append( $"{order.Date}. OrderId: {order.Id}. UserId: {order.UserId}.");

                            foreach(var detail in order.Details)
                            {
                                sb.Append($"{detail.WatchId} ({detail.UnitPrice})");
                            }

                            await admin.SendTextMessageAsync(update.Message.From.Id, sb.ToString(), replyMarkup: keyboard);
                        }
                    }

                    else
                    {
                        await admin.SendTextMessageAsync(update.Message.From.Id, "Not found");
                    }
                }
            }
        }

        private static Watch? newWatch;

        private async Task CreateNewWatch(Update update)
        {
            newWatch = new Watch();

            await SendAddNewKeyboard(update);
        }

        private async Task SendAddNewKeyboard(Update? update)
        {
            if(update == null)
            {
                return;
            }

            if (newWatch != null)
            {
                ChatId chatId;

                if (update != null && update.Message != null && update.Message.From != null)
                {
                    chatId = update.Message.From.Id;
                }
                else if (update != null && update.CallbackQuery != null && update.CallbackQuery.From != null)
                {
                    chatId = update.CallbackQuery.From.Id;
                }
                else
                {
                    return;
                }

                var row1 = new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData($"Producer{(producer != null ? $" ({producer.ProducerName})" : "")}", Enum.GetName<ReplyCodes>(ReplyCodes.GetProducers)!),
                        InlineKeyboardButton.WithCallbackData($"Model{(newWatch.Title != null ? $" ({newWatch.Title})" : "")}", Enum.GetName<ReplyCodes>(ReplyCodes.SetTitle)!)
                    };
                var row2 = new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData($"Price ({newWatch.Price})", Enum.GetName<ReplyCodes>(ReplyCodes.SetPrice)!),
                        InlineKeyboardButton.WithCallbackData($"Category{(category != null ? $" ({category.CategoryName})" : "")}", Enum.GetName<ReplyCodes>(ReplyCodes.GetCategories)!)
                    };
                var row3 = new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData($"Create", Enum.GetName<ReplyCodes>(ReplyCodes.CreateNewWatch)!),
                    };
                InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>() { row1, row2, row3 });

                await admin.SendTextMessageAsync(chatId, "Select", replyMarkup: keyboard);
            }

        }


        private async Task CreateWatch(Update update)
        {
            if (newWatch != null)
            {
                newWatch.Producer = producer;
                newWatch.Category = category;
                await context.Watches.CreateAsync(newWatch);
            }
        }

        private async Task SendCreateNewTitleMessage(Update update)
        {
            if (update != null)
            {
                if (update.CallbackQuery != null && update.CallbackQuery.From != null)
                {
                    await admin.SendTextMessageAsync(update.CallbackQuery.From.Id, "Write /model [value]");
                }
            }
        }

        private async Task SendMessage(Update update)
        {
            if (update != null)
            {
                if (update.Message != null && update.Message.Text != null)
                {
                    await admin.SendTextMessageAsync(chat, update.Message.Text);
                }
            }
        }

        private async Task SendMessageToAll(Update update)
        {
            if (update != null)
            {
                if (update.Message != null && update.Message.Text != null)
                {
                    var msg = update.Message.Text.Substring("/writeall".Length).Trim();
                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        return;
                    }
                    (await context.Users.GetAsync()).ForEach(async u => await admin.SendTextMessageAsync(u.ChatId, update.Message.Text));
                }
            }
        }

        private async Task CreateTitle(Update update)
        {
            if (update != null && update.Message != null && update.Message.Text != null)
            {
                var name = update.Message.Text.Substring("/model".Length).Trim();
                if (newWatch != null)
                {
                    newWatch.Title = name;
                }
            }

            await SendAddNewKeyboard(update);
        }

        private async Task SendCreateNewPriceMessage(Update update)
        {
            if (update != null)
            {
                if (update.CallbackQuery != null && update.CallbackQuery.From != null)
                {
                    await admin.SendTextMessageAsync(update.CallbackQuery.From.Id, "Write /price [value]");
                }
            }
        }

        private async Task CreatePrice(Update update)
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

            await SendAddNewKeyboard(update);
        }

        private async Task SendCategoriesKeyboard(Update update)
        {
            if (update != null)
            {
                if (update.CallbackQuery != null && update.CallbackQuery.From != null)
                {
                    await admin.SendCategoriesKeyboardAsync(update.CallbackQuery.From.Id, await context.Categories.GetAsync(), true);
                }
            }
        }

        private async Task SendCreateNewCategoryMessage(Update update)
        {
            await admin.SendCallbackQueryResponseAsync(update, "Write /category [name]");
        }

        private async Task CreateCategory(Update update)
        {
            if (update != null && update.Message != null && update.Message.Text != null)
            {
                var name = update.Message.Text.Substring("/category".Length).Trim();
                await context.Categories.CreateAsync(new Category()
                {
                    CategoryName = name
                });
            }

            await SendAddNewKeyboard(update);
        }

        private async Task SendProducersKeyboard(Update update)
        {
            if (update != null && update.CallbackQuery != null && update.CallbackQuery.From != null)
            {
                await admin.SendProducersKeyboardAsync(update.CallbackQuery.From.Id, await context.Producers.GetAsync(), true);
            }
        }

        private async Task SendCreateNewProducerMessage(Update update)
        {
            await admin.SendCallbackQueryResponseAsync(update, "Write /producer [name]");
        }

        private async Task CreateProducer(Update update)
        {
            if (update != null && update.Message != null && update.Message.Text != null)
            {
                var name = update.Message.Text.Substring("/producer".Length).Trim();
                await context.Producers.CreateAsync(new Producer()
                {
                    ProducerName = name
                });
            }
            await SendAddNewKeyboard(update);
        }
    }
}
