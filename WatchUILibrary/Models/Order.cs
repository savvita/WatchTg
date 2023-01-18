using WatchDb.DataAccess.Models;

namespace WatchUILibrary.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public Status? Status { get; set; }

        public DateTime Date { get; set; }

        public List<OrderDetail> Details { get; set; } = new List<OrderDetail>();

        public Order()
        {
        }

        public Order(OrderModel model)
        {
            Id = model.Id;
            Date = model.Date;
            UserId = model.UserId;

            if (model.Status != null)
            {
                Status = new Status(model.Status);
            }

        }
    }
}
