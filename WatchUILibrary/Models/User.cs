using System.ComponentModel.DataAnnotations;
using WatchDb.DataAccess.Models;

namespace WatchUILibrary.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public long ChatId { get; set; }

        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }

        public User()
        {
        }

        public User(UserModel model)
        {
            Id = model.Id;
            ChatId = model.ChatId;
            Username = model.Username;
            FirstName = model.FirstName;
            Login = model.Login;
            Password = model.Password;
        }
    }
}
