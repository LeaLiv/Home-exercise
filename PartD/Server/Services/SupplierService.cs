
using PartD.Interfaces;
using PartD.Models;


namespace PartD.Services;

public class SupplierService : IService<Supplier>
{

    List<Supplier> _suppliers;
    const string COLLECTION_NAME = "Suppliers";

    public SupplierService()
    {
        _suppliers = MongoService.getCollection<Supplier>(COLLECTION_NAME).Result;
        foreach (var supplier in _suppliers)
        {
            // Console.WriteLine($"Supplier: {supplier}");
        }

    }
    public List<Supplier> GetAll() => _suppliers;

    public Supplier Get(string phone)
    {
        return _suppliers.FirstOrDefault(s => s.Phone == phone);
    }
    public List<Supplier> GetAllItemsByfilter(Predicate<Supplier> filter) =>
    _suppliers.FindAll(o => filter(o));
    public void Insert(Supplier newItem)
    {
        _suppliers.Add(newItem);
        MongoService.InsertOne(newItem, COLLECTION_NAME).Wait();

    }
}
public static class SupplierUtilities
{
    public static void AddSupplier(this IServiceCollection services)
    {
        services.AddSingleton<IService<Supplier>, SupplierService>();
    }
}