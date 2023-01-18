using System.ComponentModel.DataAnnotations;
using WatchDb.DataAccess.Models;

namespace WatchUILibrary.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public long ChatId { get; set; }

        public User()
        {
        }

        public User(UserModel model)
        {
            Id = model.Id;
            ChatId = model.ChatId;
        }
    }
}
