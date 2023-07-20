using CategoriesApp.API.Entities;
using Microsoft.Extensions.Options;
using System.Linq;

namespace CategoriesApp.API
{
    public class CategoryService : ICategoryService
    {
        private readonly CategoriesContext _context;
        public Dictionary<string, Category> CategoryDict { get; set; }
        public readonly ConstantsConf DepthLimit;

        public CategoryService(CategoriesContext context, IOptions<ConstantsConf> options)
        {
            _context = context;
            CategoryDict = new Dictionary<string, Category>();
            DepthLimit = options.Value;
            var categories = _context.Categories.ToList();
           
            foreach (var category in categories)
            {
                CategoryDict.Add(category.Name, category);
            }

        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public void AddUpdateCategories(List<CategoryTuple> catTuples)
        {
            foreach (var item in catTuples)
            {
                if (!CategoryDict.ContainsKey(item.Parent) 
                    && !CategoryDict.ContainsKey(item.Child))
                {
                    GenerateCategoryFromTuple(item);
                }

                if(CategoryDict.ContainsKey(item.Child) && CategoryDict[item.Child].IsRoot)
                {
                    var possibleParent = CategoryDict.ContainsKey(item.Parent)? CategoryDict[item.Parent]: null;
                    if (possibleParent != null && possibleParent.ParentCategoryId != null && IsAncestor(item.Child, possibleParent))
                    {
                        continue;
                    }
                    GenerateCategoryFromTuple(item, possibleParent, CategoryDict[item.Child]);
                }
                else if (!CategoryDict.ContainsKey(item.Child))
                {
                    GenerateCategoryFromTuple(item, CategoryDict[item.Parent], null);
                }

            }

        }

        private void GenerateCategoryFromTuple(CategoryTuple item, Category? parentParam = null, Category? childParam = null)
        {

            if(childParam == null)
            {
                if (parentParam != null && SizeUp(parentParam) == DepthLimit.Depth)
                    return;

                childParam = new Category
                {
                    Name = item.Child,
                    ChildCategories = new List<Category>()
                };
                CategoryDict.Add(childParam.Name, childParam);
                _context.Categories.Add(childParam);
            }

            if (parentParam == null)
            {
                if (SizeDown(childParam) == DepthLimit.Depth)
                    return;

                var parent = new Category
                {
                    Name = item.Parent,
                    ChildCategories = new List<Category> { childParam },
                    IsRoot = true
                };

                _context.Categories.Add(parent);

                CategoryDict.Add(parent.Name, parent);
            }
            else
            {
                
                parentParam.ChildCategories.Add(childParam);
            }
            _context.SaveChanges();
        }

        private bool IsAncestor(string possibleAncestor, Category category)
        {
            var parent = _context.Categories.FirstOrDefault(c => c.Id == category.ParentCategoryId);
            if(parent == null) return false;
            else if(parent.IsRoot) return parent.Name == possibleAncestor;
            else if(parent.Name == possibleAncestor) return true;
            else return IsAncestor(possibleAncestor, parent);
        }

        private int SizeUp(Category category)
        {
            var parent = _context.Categories.FirstOrDefault(c => c.Id == category.ParentCategoryId);
            if (parent == null || parent.IsRoot) return 1;
            else return 1 + SizeUp(parent);
        }

        private int SizeDown(Category category)
        {
            if (category == null) return 0;
            var sizeDown = 1;
            while(category.ChildCategories.Count() != 0)
            {
                sizeDown += SizeDown(category.ChildCategories[0]);
            }
            return sizeDown;
        }
    }
}
