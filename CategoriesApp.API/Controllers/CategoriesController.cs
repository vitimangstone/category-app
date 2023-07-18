using CategoriesApp.API.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CategoriesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;

        }
        // GET: api/<CategoriesController>
        [HttpGet]
        public IActionResult Get()
        {
            var categories = _service.GetCategories();
            return Ok(categories);
        }

        // POST api/<CategoriesController>
        [HttpPost]
        public void Post([FromBody] List<CategoryTuple> tuples)
        {
            _service.AddUpdateCategories(tuples);
        }

    }
}
