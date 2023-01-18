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
                    await AddUserAsync(update.CallbackQuery.From.Id);

                    if (Enum.TryParse<ReplyCodes>(update.CallbackQuery.Data, out ReplyCodes code))
                    {
                        if (replyCodesHandles.ContainsKey(code))
                        {
                            await replyCodesHandles[code](update);
                        }
                    }
                    else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.CategoryId)!))
                    {
                        if(TgHelper.GetIntFromCallbackQuery(update, ReplyCodes.CategoryId, out int id))
                        {
                            await SendWatchesByCategoryAsync(update.CallbackQuery.From.Id, id);
                        }
                    }
                    else if (update.CallbackQuery.Data.StartsWith(Enum.GetName<ReplyCodes>(ReplyCodes.ProducerId)!))
                    {
                        if (TgHelper.GetIntFromCallbackQuery(update, ReplyCodes.ProducerId, out int id))
                        {
                            await SendWatchesByProducerAsync(update.CallbackQuery.From.Id, id);
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
            }
            return Results.Ok();
        }

        private async Task HandleCommandAsync(Update update)
        {
            await AddUserAsync(update.Message!.From!.Id);

            string command = TgHelper.GetCommand(update.Message.Text!);

            if (commandHandles.ContainsKey(command))
            {
                await commandHandles[command](update);
            }
        }

        private async Task AddUserAsync(ChatId chat)
        {
            var user = await context.Users.GetAsync((long)chat.Identifier!);

            if (user == null)
            {
                await context.Users.CreateAsync(new WatchUILibrary.Models.User()
                {
                    ChatId = (long)chat.Identifier!
                });
            }
        }

        // Keyboards
        private async Task SendFiltersKeyboardAsync(Update update)
        {
            if (update != null && update.Message != null && update.Message.From != null)
            {
                await bot.SendFiltersKeyboard(update.Message.From.Id);
            }
        }

        private async Task SendCategoriesKeyboardAsync(Update update)
        {
            if (update != null && update.CallbackQuery != null && update.CallbackQuery.From != null)
            {
                await bot.SendCategoriesKeyboardAsync(update.CallbackQuery.From.Id, await context.Categories.GetAsync());
            }
        }

        private async Task SendProducersKeyboardAsync(Update update)
        {
            if (update != null && update.CallbackQuery != null && update.CallbackQuery.From != null)
            {
                await bot.SendProducersKeyboardAsync(update.CallbackQuery.From.Id, await context.Producers.GetAsync());
            }
        }

        // =================================

        private async Task SendWatchesAsync(Update update)
        {
            if (update != null && update.Message != null && update.Message.From != null)
            {
                var watches = await context.Watches.GetAsync();
                await bot.SendWatchesAsync(update.Message.From.Id, watches);
            }
        }

        private async Task SendWatchesByCategoryAsync(ChatId chat, int categoryId)
        {
            var watches = (await context.Watches.GetAsync()).Where(x => x.Category != null && x.Category.Id == categoryId);

            await bot.SendWatchesAsync(chat, watches);
        }

        private async Task SendWatchesByProducerAsync(ChatId chat, int producerId)
        {
            var watches = (await context.Watches.GetAsync()).Where(x => x.Producer != null && x.Producer.Id == producerId);

            await bot.SendWatchesAsync(chat, watches);
        }

        private async Task FilterWatchesAsync(Update update)
        {
            if (update != null && update.Message != null && update.Message.Text != null && update.Message.From != null)
            {
                var model = TgHelper.GetValueFromCommand(update, "/model");
                var watches = await context.Watches.GetAsync(model);

                await bot.SendWatchesAsync(update.Message.From.Id, watches);
            }
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
            await bot.SendCallbackQueryResponseAsync(update, "Write /model [value]");
        }


    }
}
