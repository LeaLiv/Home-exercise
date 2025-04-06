
namespace PartD.Interfaces;

public interface IService<T>
{
    List<T> GetAll();

    T Get(string Id);

    void Insert(T newItem);


}