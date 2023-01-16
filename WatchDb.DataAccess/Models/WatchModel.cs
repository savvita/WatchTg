namespace WatchDb.DataAccess.Models
{
    public class WatchModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int? CategoryId { get; set; }
        public int? ProducerId { get; set; }
        public decimal Price { get; set; }
        public int Available { get; set; }
        public bool OnSale { get; set; }
    }
}
