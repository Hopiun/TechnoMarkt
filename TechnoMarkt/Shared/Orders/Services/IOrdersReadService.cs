using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechnoMarkt.Shared.Orders.Services
{
    public interface IOrdersReadService<TFilter>
    {
        Task<List<OrdersTableRow>> GetOrdersAsync(int id, TFilter? filter);
    }
}
