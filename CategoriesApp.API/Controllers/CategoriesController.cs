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
            var cats = CreateDictionaries(categories.Where(c => c.IsRoot).ToList());
            return Ok(cats);
        }

        // POST api/<CategoriesController>
        [HttpPost]
        public void Post([FromBody] object obj)
        {
            var categories = new List<CategoryTuple>();
            var objString = obj.ToString();
            var tuples = objString.Split(',');
            foreach (var tuple in tuples)
            {
                var tupleValues = tuple.Split(':');
                categories.Add(new CategoryTuple
                {
                    Parent = tupleValues[0].Replace("{","").Replace("}","").Replace("\"", "").Trim(),
                    Child = tupleValues[1].Replace("{", "").Replace("}", "").Replace("\"", "").Trim()
                });
            }

            _service.AddUpdateCategories(categories);
        }

        private Dictionary<string, object> CreateDictionaries(List<Category> categories)
        {
            var dictInit = new Dictionary<string, object>();

            foreach (var category in categories)
            {
                if (category.ChildCategories.Count() == 0)
                {
                    dictInit.Add(category.Name, new Dictionary<string, object>());
                }
                else
                {
                    var dictObj = CreateDictionaries(category.ChildCategories);
                    dictInit.Add(category.Name, dictObj);
                }

            }
            return dictInit;

        }

    }
}
