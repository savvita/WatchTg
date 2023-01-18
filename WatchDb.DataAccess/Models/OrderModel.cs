namespace WatchDb.DataAccess.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }
        public StatusModel? Status { get; set; }
    }
}
