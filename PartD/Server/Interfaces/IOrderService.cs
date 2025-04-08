
using partD.Models;

namespace PartD.Interfaces;

public interface IOrderService
{
    List<Order> GetAll();

    Order Get(string Id);

    void Insert(Order newItem);

    List<Order> GetAllItemsByfilter(Predicate<Order> filter);

    public void UpdateStatus(string orderId,string status);

}