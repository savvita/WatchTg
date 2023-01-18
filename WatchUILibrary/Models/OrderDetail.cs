using WatchDb.DataAccess.Models;

namespace WatchUILibrary.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        public int WatchId { get; set; }

        public decimal UnitPrice{ get; set; }

        public OrderDetail()
        {
        }

        public OrderDetail(OrderDetailModel model)
        {
            Id = model.Id;
            UnitPrice = model.UnitPrice;
            WatchId = model.WatchId;
        }
    }
}
