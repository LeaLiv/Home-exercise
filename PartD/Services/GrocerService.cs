
// using PartD.Interfaces;

// namespace PartD.Services;

// public class GrocerService : IService<Grocer>
// {
//     private List<Models.Grocer> _grocers = new List<Models.Grocer>();

//     public List<Models.Grocer> GetAll()
//     {
//         return _grocers;
//     }

//     public Models.Grocer Get(int Id)
//     {
//         return _grocers.FirstOrDefault(g => g.Id == Id);
//     }

//     public void Insert(Models.Grocer newItem)
//     {
//         _grocers.Add(newItem);
//     }

// }