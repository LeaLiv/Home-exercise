
using Microsoft.AspNetCore.Mvc;
using partD.Models;
using PartD.Interfaces;


namespace PartD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("GetAllOrders")]
        public ActionResult<List<Order>> GetAll() =>
             _orderService.GetAll();

        [HttpGet("GetAllOrdersToSupplier/{supplierPhone}")]
        public ActionResult<List<Order>> GetAllOrdersToSupplier(string supplierPhone) =>
             _orderService.GetAllItemsByfilter(s => s.SupplierId == supplierPhone);

        [HttpGet("GetAllOrdersToGrocer/{grocerId}")]
        public ActionResult<List<Order>> GetAllOrdersToGrocer(string grocerId) =>
             _orderService.GetAllItemsByfilter(s => s.GrocerId == grocerId);


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
        [HttpPost("MakeOrder")]
        public IActionResult Insrt(Order newOrder)
        {
            _orderService.Insert(newOrder);
            return CreatedAtAction(nameof(Get), new { id = newOrder._id }, newOrder);
        }

        [HttpPut("UpdateOrderStatus/{id}")]
        public IActionResult Update(string id, [FromBody] UpdateStatusRequest status)
        {
            if (Get(id) == null)
            {
                return NotFound();
            }
            _orderService.UpdateStatus(id, status.Status);
            return NoContent();
        }
    }
}
public class UpdateStatusRequest
{
    public string Status { get; set; }
}