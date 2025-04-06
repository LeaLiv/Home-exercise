using MongoDB.Driver;
using partD.Models;
using PartD.Interfaces;

namespace PartD.Services;

public class OrderService:IService<Order>
{       
    List<Order> _orders;

    public OrderService()
    {
        _orders = MongoService.getCollection<Order>("Orders").Result;
        foreach (var order in _orders)
        {
            Console.WriteLine($"Order: {order}");
        }
    }
    public Order Get(string Id)=> _orders.FirstOrDefault(o => o.Id == Id);

    public List<Order> GetAll() => _orders;



    public void Insert(Order newItem)
    {
        Mon addToDB(newItem);
    }
}