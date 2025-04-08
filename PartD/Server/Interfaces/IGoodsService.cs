
using PartD.Models;

namespace PartD.Interfaces;

public interface IGoodsService
{
    List<Goods> GetAll();

    Goods Get(string itemId);

    void Insert(Goods newItem);
    List<Goods> GetAllItemsByfilter(Predicate<Goods> filter);
    void Update(string itemId, string field, string value);
}