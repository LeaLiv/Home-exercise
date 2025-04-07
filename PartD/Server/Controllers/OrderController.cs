
using Microsoft.AspNetCore.Mvc;
using partD.Models;
using PartD.Interfaces;


namespace PartD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        IService<Order> _orderService;
        public OrderController(IService<Order> orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("GetAllOrders")]
        public ActionResult<List<Order>> GetAll() =>
             _orderService.GetAll();

        [HttpGet("GetAllOrdersToSupplier/{supplierPhone}")]
        public ActionResult<List<Order>> GetAllOrdersToSupplier(string supplierPhone) =>
             _orderService.GetAllItemsByfilter(s => s.SupplierId == supplierPhone);

        [HttpGet("GetOrderById/{id}")]
        public ActionResult<Order> Get(string id)
        {
            var order = _orderService.Get(id);
            if (order == null)
            {
                return NotFound();
            }
            return order;
        }
        [HttpPost]
        public IActionResult Insert(Order newOrder)
        {
            _orderService.Insert(newOrder);
            return CreatedAtAction(nameof(Get), new { id = newOrder._id }, newOrder);
        }
    }
}