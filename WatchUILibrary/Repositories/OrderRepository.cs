using WatchDb.DataAccess.Models;
using WatchDb.DataAccess.Repositories;
using WatchUILibrary.Models;

namespace WatchUILibrary.Repositories
{
    public class OrderRepository
    {
        private DbContext context;
        public OrderRepository(DbContext context)
        {
            this.context = context;
        }
        public async Task<List<Order>> GetAsync()
        {
            var models = await context.Orders.GetAsync();

            List<Order> orders = new List<Order>();

            foreach(var model in models)
            {
                var user = await context.Users.GetAsync(model.UserId);

                var details = (await context.OrderDetails.GetByOrderId(model.Id)).Select(model => new OrderDetail(model)).ToList();
                orders.Add(new Order(model)
                {
                    Details = details
                });
            }

            return orders;
        }

        public async Task<Order?> GetAsync(int id)
        {
            var model = await context.Orders.GetAsync(id);

            return model != null ? new Order(model) : null;
        }

        public async Task<List<Order>> GetAsync(int? userId, int? statusId)
        {
            return (await context.Orders.GetAsync(userId, statusId)).Select(model => new Order(model)).ToList();
        }

        public async Task<Order?> UpdateAsync(Order order)
        {
            var model = await context.Orders.UpdateAsync(new OrderModel()
            {
                Id = order.Id,
                Date = order.Date,
                StatusId = order.Status != null ? order.Status.Id : 0,
                UserId = order.UserId
            });

            if (model == null)
            {
                return null;
            }

            return new Order(model);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await context.Orders.DeleteAsync(id);
        }

        public async Task<Order> CreateAsync(Order order)
        {
            var details = order.Details;
            order = new Order(await context.Orders.CreateAsync(new OrderModel()
            {
                Date = order.Date,
                StatusId = order.Status != null ? order.Status.Id : 0,
                UserId = order.UserId,
            }))
            {
                Details = details
            };
            order.Details.ForEach(async detail => await context.OrderDetails.CreateAsync(new OrderDetailModel()
            {
                OrderId = order.Id,
                WatchId = detail.WatchId,
                //TODO add to modelCount = detail.
                UnitPrice = detail.UnitPrice
            }));
            return order;
        }

        public async Task<bool> CloseOrderAsync(int id)
        {
            var order = await context.Orders.GetAsync(id);

            if (order != null)
            {
                order.StatusId = 2;
                await context.Orders.UpdateAsync(order);
                return true;
            }

            return false;
        }
    }
}
