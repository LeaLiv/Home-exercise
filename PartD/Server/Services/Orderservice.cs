using MongoDB.Driver;
using partD.Models;
using PartD.Interfaces;

namespace PartD.Services;

public class OrderService:IService<Order>
{       
    List<Order> _orders;
    const string COLLECTION_NAME = "Orders";
    public OrderService()
    {
        _orders = MongoService.getCollection<Order>(COLLECTION_NAME).Result;
        foreach (var order in _orders)
        {
            Console.WriteLine($"Order: {order}");
        }
    }
    public Order Get(string Id)=> _orders.FirstOrDefault(o => o._id == Id);

    public List<Order> GetAll() => _orders;

    public List<Order> GetAllItemsByfilter(Predicate<Order> filter)
    {
        Console.WriteLine($"Filter: {filter}");
        return _orders.FindAll(o => filter(o));
    }

    public void Insert(Order newItem)
    {
        _orders.Add(newItem);
        MongoService.InsertOne(newItem, COLLECTION_NAME).Wait();
    }
}
public static class OrderUtilities
{
    public static void AddOrderService(this IServiceCollection services)
    {
        services.AddSingleton<IService<Order>, OrderService>();
    }
}