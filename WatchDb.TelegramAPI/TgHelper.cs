using Telegram.Bot.Types;

namespace WatchDb.TelegramAPI
{
    public static class TgHelper
    {
        public static string GetValueFromCommand(Update update, string command)
        {
            if (update != null && update.Message != null && update.Message.Text != null)
            {
                var model = update.Message.Text.Substring(command.Length).Trim();
            }

            return string.Empty;
        }

        public static bool GetIntFromCallbackQuery(Update update, ReplyCodes code, out int id)
        {
            if (update != null && update.CallbackQuery != null && update.CallbackQuery.Data != null)
            {
                if (int.TryParse(update.CallbackQuery.Data
                   .Substring(Enum.GetName<ReplyCodes>(code)!.Length).Trim(), out int _id))
                {
                    id = _id;
                    return true;
                }
            }
            id = -1;
            return false;
        }

        public static string GetCommand(string msg)
        {
            string command;

            var idx = msg.IndexOf(' ');

            if (idx > 0)
            {
                command = msg.Substring(0, idx).ToLower();
            }
            else
            {
                command = msg.ToLower();
            }

            return msg;
        }
    }
}
