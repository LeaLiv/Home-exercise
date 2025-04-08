

using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using partD.Models;
using PartD.Interfaces;
using PartD.Models;
using PartD.Services;

namespace PartD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoodsController : ControllerBase
    {
        IGoodsService _goodsService;
        public GoodsController(IGoodsService GoodsService)
        {
            _goodsService = GoodsService;
        }
        [HttpGet("GetAllGoods")]
        public ActionResult<List<Goods>> GetAll() =>
             _goodsService.GetAll();

        [HttpGet("GetGoodsByName/{name}")]
        public ActionResult<Goods> Get(string name)
        {
            var Goods = _goodsService.Get(name);
            if (Goods == null)
            {
                return NotFound();
            }
            return Goods;
        }
        [HttpPost]
        public IActionResult Insert(Goods newGoods)
        {
            _goodsService.Insert(newGoods);
            return CreatedAtAction(nameof(Get), new { phone = newGoods.ItemId }, newGoods);
        }
        [HttpPut("UpdateGoods/{itemId}")]
        public IActionResult Update(Goods goods, string itemId)
        {
            if (goods == null || goods.ItemId != itemId)
            {
                return NotFound();
            }
            _goodsService.Update(goods.ItemId, "count", goods.Count.ToString());
            Console.WriteLine($"Updated Goods: {goods.ItemId} - {goods.Count}");
            return NoContent();
        }
        [HttpPut("UpdateGoodsSupply")]
        public IActionResult UpdateGoods(Dictionary<string, int> goods)
        {
            List<string> itemNotFound = new List<string>();
            IOrderService _orderService = new OrderService();
            IService<Supplier> _supplierService = new SupplierService();
            var suppliers = _supplierService.GetAll();
            foreach (var prop in goods)
            {
                Console.WriteLine($"Property: {prop.Key}, Value: {prop.Value}");
                ActionResult<Goods> item = Get(prop.Key);
                if (item != null)
                {
                    //update the count in db
                    _goodsService.Update(item.Value.ItemId, "count", prop.Value.ToString());
                    item = Get(prop.Key);
                    if (item.Value.Count < item.Value.MinCount)
                    {
                        //need to order supply
                        Supplier supplierToOrder = null;
                        Product prodToOrder = null;
                        foreach (var supplier in suppliers)
                        {
                            var prod = supplier.Products.FirstOrDefault(p => p.productName == prop.Key);
                            //if the supplier has the item
                            if (prod != null)
                            {
                                if (prodToOrder == null || prod.pricePerItem < prodToOrder.pricePerItem)
                                {
                                    prodToOrder = prod;
                                    supplierToOrder = supplier;
                                }
                            }
                        }
                        if (prodToOrder == null)
                        {
                            itemNotFound.Add(prop.Key);
                        }
                        else
                        {
                            _orderService.Insert(new Order
                            {
                                _id = ObjectId.GenerateNewId().ToString(),

                                status = "ממתינה",
                                SupplierId = supplierToOrder._id,
                                GrocerId = "9a8c6f2f-4d12-1dbc-981f-79aaae33c8cf",
                                Products = [prodToOrder]
                            });
                        }
                    }
                }
            }
            return Ok(new { message = "Goods not ordered", itemsNotFound = itemNotFound });
        }
    }
}

