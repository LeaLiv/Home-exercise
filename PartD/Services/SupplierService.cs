
using PartD.Interfaces;
using PartD.Models;
using MongoDB.Driver;
using System.Collections.ObjectModel;

namespace PartD.Services;

public class SupplierService : IService<Supplier>
{
    public async Task<List<Supplier>> GetSupplier()
    {
        MongoClient client = new MongoClient("mongodb+srv://leahl11730:Lea0195!@cluster0.qxme0.mongodb.net/");
        var database = client.GetDatabase("Groceries");
        IMongoCollection<Supplier> Collection = database.GetCollection<Supplier>("Suppliers");
        return await Collection.Find(_ => true).ToListAsync();
    }
    List<Supplier> _suppliers;// = new List<Supplier>();

    public SupplierService()
    {
        _suppliers = GetSupplier().Result;
        Console.WriteLine($"Connected to MongoDB ");
        foreach (var supplier in _suppliers)
        {
            Console.WriteLine($"Supplier: {supplier}");
        }

    }
    public List<Supplier> GetAll() => _suppliers;

    public Supplier Get(string Id)
    {
        return _suppliers.FirstOrDefault(s => s._id == Id);
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