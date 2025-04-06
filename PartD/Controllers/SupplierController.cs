
using Microsoft.AspNetCore.Mvc;
using PartD.Interfaces;
using PartD.Models;

namespace PartD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SupplierController : ControllerBase
    {
        IService<Supplier> _supplierService;
        public SupplierController(IService<Supplier> supplierService)
        {
            _supplierService = supplierService;
        }
        [HttpGet]
        public ActionResult<List<Supplier>> GetAll() =>
             _supplierService.GetAll();

        [HttpGet("{phone}")]
        public ActionResult<Supplier> Get(string phone)
        {
            var supplier = _supplierService.Get(phone);
            if (supplier == null)
            {
                return NotFound();
            }
            return supplier;
        }
        [HttpPost]
        public IActionResult Insert(Supplier newSupplier)
        {
            _supplierService.Insert(newSupplier);
            return CreatedAtAction(nameof(Get), new { id = newSupplier._id }, newSupplier);
        }
    }
}