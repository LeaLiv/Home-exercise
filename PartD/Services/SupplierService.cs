
using PartD.Interfaces;
using PartD.Models;


namespace PartD.Services;

public class SupplierService : IService<Supplier>
{

    List<Supplier> _suppliers;

    public SupplierService()
    {
        _suppliers = MongoService.getCollection<Supplier>("Suppliers").Result;
        foreach (var supplier in _suppliers)
        {
            Console.WriteLine($"Supplier: {supplier}");
        }

    }
    public List<Supplier> GetAll() => _suppliers;

    public Supplier Get(string phone)
    {
        return _suppliers.FirstOrDefault(s => s.Phone == phone);
    }

    public void Insert(Supplier newItem)
    {
        _suppliers.Add(newItem);
    }
}
public static class SupplierUtilities
{
    public static void AddSupplier(this IServiceCollection services)
    {
        services.AddSingleton<IService<Supplier>, SupplierService>();
    }
}