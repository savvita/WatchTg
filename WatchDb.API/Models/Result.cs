namespace WatchDb.API.Models
{
    public class Result<T>
    {
        public JWTToken? Token { get; set; }
        public T? Value { get; set; }
    }
}
