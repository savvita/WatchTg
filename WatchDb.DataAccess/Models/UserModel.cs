namespace WatchDb.DataAccess.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
    }
}
