
using PartD.Interfaces;
using PartD.Models;

namespace PartD.Services;
public class GoodsService : IGoodsService
{
    List<Goods> _goods;
    const string COLLECTION_NAME = "Goods";
    public GoodsService()
    {
        _goods = MongoService.getCollection<Goods>(COLLECTION_NAME).Result;
    }
    public List<Goods> GetAll() => _goods;

    public Goods Get(string itemName)
    {
        return _goods.FirstOrDefault(s => s.Name == itemName);
    }
    public List<Goods> GetAllItemsByfilter(Predicate<Goods> filter) =>
    _goods.FindAll(o => filter(o));
    public void Insert(Goods newItem)
    {
        _goods.Add(newItem);
        MongoService.InsertOne(newItem, COLLECTION_NAME).Wait();
    }
    public void Update(string itemId, string field, string value)
    {
        MongoService.UpdateOne<Goods>(itemId, value, field, COLLECTION_NAME).Wait();
    }
}
public static class GoodsUtilities
{
    public static void AddGoods(this IServiceCollection services)
    {
        services.AddSingleton<IGoodsService, GoodsService>();
    }
}