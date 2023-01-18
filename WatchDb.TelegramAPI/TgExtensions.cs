using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WatchUILibrary.Models;

namespace WatchDb.TelegramAPI
{
    public static class TgExtensions
    {
        public async static Task SendWatchesAsync(this TelegramBotClient client, ChatId chat, IEnumerable<Watch> watches, bool isAdmin = false)
        {
            if (watches.Count() > 0)
            {

                foreach (var watch in watches)
                {
                    InlineKeyboardMarkup keyboard;

                    if (isAdmin)
                    {
                        keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Delete", $"{Enum.GetName<ReplyCodes>(ReplyCodes.DeleteWatch)} {watch.Id}"));
                    }
                    else
                    {
                        keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Buy", $"{Enum.GetName<ReplyCodes>(ReplyCodes.Buy)} {watch.Id}"));
                    }
                    string url = "https://bpgroup.lv/i/product_images/images/Z2000128425.jpg";
                    await client.SendPhotoAsync(chat, new Telegram.Bot.Types.InputFiles.InputOnlineFile(url));
                    await client.SendTextMessageAsync(chat, $"{(watch.Producer != null ? watch.Producer.ProducerName : "")} {watch.Title} ({watch.Price})", replyMarkup: keyboard);
                }
            }

            else
            {
                await client.SendTextMessageAsync(chat, "Not found");
            }
        }

        public static async Task SendUsersAsync(this TelegramBotClient client, ChatId chat, IEnumerable<WatchUILibrary.Models.User> users)
        {
            if (users.Count() > 0)
            {

                foreach (var user in users)
                {
                    InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Write", $"{Enum.GetName<ReplyCodes>(ReplyCodes.WriteToUser)} {user.Id}"));

                    string url = "https://bpgroup.lv/i/product_images/images/Z2000128425.jpg";
                    await client.SendPhotoAsync(chat, new Telegram.Bot.Types.InputFiles.InputOnlineFile(url));
                    await client.SendTextMessageAsync(chat, $"{user.ChatId}", replyMarkup: keyboard);
                }
            }

            else
            {
                await client.SendTextMessageAsync(chat, "Not found");
            }
        }

        public static async Task SendOrdersAsync(this TelegramBotClient client, ChatId chat, IEnumerable<Order> orders)
        {
            if (orders.Count() > 0)
            {
                foreach (var order in orders)
                {
                    InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Close", $"{Enum.GetName<ReplyCodes>(ReplyCodes.CloseOrder)} {order.Id}"));

                    StringBuilder sb = new StringBuilder();
                    sb.Append($"{order.Date}. OrderId: {order.Id}. UserId: {order.UserId}.");

                    foreach (var detail in order.Details)
                    {
                        sb.Append($"{detail.WatchId} ({detail.UnitPrice})");
                    }

                    await client.SendTextMessageAsync(chat, sb.ToString(), replyMarkup: keyboard);
                }
            }

            else
            {
                await client.SendTextMessageAsync(chat, "Not found");
            }

        }

        public static async Task SendAddNewWatchKeyboardAsync(this TelegramBotClient client, ChatId chat, Category? category, Producer? producer, Watch watch)
        {
            var row1 = new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData($"Producer{(producer != null ? $" ({producer.ProducerName})" : "")}", Enum.GetName<ReplyCodes>(ReplyCodes.GetProducers)!),
                    InlineKeyboardButton.WithCallbackData($"Model{(watch.Title != null ? $" ({watch.Title})" : "")}", Enum.GetName<ReplyCodes>(ReplyCodes.SetTitle)!)
                };
            var row2 = new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData($"Price ({watch.Price})", Enum.GetName<ReplyCodes>(ReplyCodes.SetPrice)!),
                    InlineKeyboardButton.WithCallbackData($"Category{(category != null ? $" ({category.CategoryName})" : "")}", Enum.GetName<ReplyCodes>(ReplyCodes.GetCategories)!)
                };
            var row3 = new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData($"Create", Enum.GetName<ReplyCodes>(ReplyCodes.CreateNewWatch)!),
                };
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>() { row1, row2, row3 });

            await client.SendTextMessageAsync(chat, "Select", replyMarkup: keyboard);
        }

        public static async Task SendCategoriesKeyboardAsync(this TelegramBotClient client, ChatId chat, IEnumerable<Category> categories, bool isAdmin = false)
        {
            var buttons = categories
                .Select(c => new List<InlineKeyboardButton>() {
                            InlineKeyboardButton.WithCallbackData(c.CategoryName, $"{Enum.GetName<ReplyCodes>(ReplyCodes.CategoryId)} {c.Id}")
                })
                .ToList();

            if(isAdmin)
            {
                buttons.Add(new List<InlineKeyboardButton>(){
                        InlineKeyboardButton.WithCallbackData("Add new", Enum.GetName(ReplyCodes.CreateNewCategory)!)
                    });
            }

            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);

            await client.SendTextMessageAsync(chat, "Select category", replyMarkup: keyboard);
        }

        public static async Task SendProducersKeyboardAsync(this TelegramBotClient client, ChatId chat, IEnumerable<Producer> producers, bool isAdmin = false)
        {
            var buttons = producers
                .Select(c => new List<InlineKeyboardButton>() {
                    InlineKeyboardButton.WithCallbackData(c.ProducerName, $"{Enum.GetName<ReplyCodes>(ReplyCodes.ProducerId)} {c.Id}")
                })
                .ToList();

            if(isAdmin)
            {
                buttons.Add(new List<InlineKeyboardButton>(){
                        InlineKeyboardButton.WithCallbackData("Add new", Enum.GetName(ReplyCodes.CreateNewProducer)!)
                    });
            }

            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);

            await client.SendTextMessageAsync(chat, "Select producer", replyMarkup: keyboard);
        }

        public static async Task SendFiltersKeyboard(this TelegramBotClient client, ChatId chat)
        {
            var row1 = new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Producer", Enum.GetName<ReplyCodes>(ReplyCodes.GetProducers)!),
                };
            var row2 = new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Category", Enum.GetName<ReplyCodes>(ReplyCodes.GetCategories)!)
                };
            var row3 = new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Model", Enum.GetName<ReplyCodes>(ReplyCodes.SetTitle)!)
                };

            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>() { row1, row2, row3 });

            await client.SendTextMessageAsync(chat, "Select filter", replyMarkup: keyboard);           
        }

        public static async Task SendCallbackQueryResponseAsync(this TelegramBotClient client, Update update, string msg)
        {
            if (update != null && update.CallbackQuery != null && update.CallbackQuery.From != null)
            {
                 await client.SendTextMessageAsync(update.CallbackQuery.From.Id, msg);
            }
        }

        public static async Task SendMessageResponseAsync(this TelegramBotClient client, Update update, string msg)
        {
            if (update != null && update.Message != null && update.Message.From != null)
            {
                await client.SendTextMessageAsync(update.Message.From.Id, msg);
            }
        }
    }
}
