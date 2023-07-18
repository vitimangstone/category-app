using CategoriesApp.API;
using CategoriesApp.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CategoriesApp.Test
{
    public class CategoryServiceTests
    {
        private readonly CategoriesContext _context;
        private IOptions<Constants> _options;

        public CategoryServiceTests()
        {
            var optionBuilder = new DbContextOptionsBuilder<CategoriesContext>()
                .UseInMemoryDatabase("TestinDb");

            _context = new CategoriesContext(optionBuilder.Options);
            _options = Options.Create(new Constants { Depth = 10}); ;
        }

        [Fact]
        public void AddingExistingCategoriesToDictionary()
        {
            var firstCategory = new Category
            {
                Name = "A",
                ChildCategories = new List<Category>(),
                IsRoot = true
            };
            _context.Categories.Add(firstCategory);
            _context.SaveChanges();
            var service = new CategoryService(_context, _options);

            Assert.True(service.CategoryDict["A"] == firstCategory);
        }

        [Fact]
        public void AddingChildToExistingParent()
        {
            var firstCategory = new Category
            {
                Name = "C",
                ChildCategories = new List<Category>(),
                IsRoot = true
            };
            _context.Categories.Add(firstCategory);
            _context.SaveChanges();
            var service = new CategoryService(_context, _options);

            var tuple = new CategoryTuple { Child = "B", Parent = "C" };
            service.AddUpdateCategories(new List<CategoryTuple> { tuple });

            Assert.True(service.CategoryDict["C"] == firstCategory);
            Assert.True(service.CategoryDict["B"] != null);
            Assert.True(service.CategoryDict["C"].ChildCategories.Count() > 0);
            Assert.True(service.CategoryDict["C"].ChildCategories[0] == service.CategoryDict["B"]);
            
        }

        [Fact]
        public void AddingChildToExistingParentWithSize1()
        {
            var firstCategory = new Category
            {
                Name = "P",
                ChildCategories = new List<Category>(),
                IsRoot = true
            };
            _context.Categories.Add(firstCategory);
            _context.SaveChanges();
            _options = Options.Create(new Constants { Depth = 1 }); ;
            var service = new CategoryService(_context, _options);

            var tuple = new CategoryTuple { Child = "O", Parent = "P" };
            service.AddUpdateCategories(new List<CategoryTuple> { tuple });

            Assert.False(service.CategoryDict.ContainsKey("O"));
        }

        [Fact]
        public void AddingChildAndParent()
        {
            var service = new CategoryService(_context, _options);

            var tuple = new CategoryTuple { Child = "D", Parent = "F" };
            service.AddUpdateCategories(new List<CategoryTuple> { tuple });

            Assert.True(service.CategoryDict["F"] != null);
            Assert.True(service.CategoryDict["D"] != null);
            Assert.True(service.CategoryDict["F"].ChildCategories.Count() > 0);
            Assert.True(service.CategoryDict["F"].ChildCategories[0] == service.CategoryDict["D"]);
        }

        [Fact]
        public void AddingParentToExistingChildNoRoot()
        {
            var firstCategory = new Category
            {
                Name = "V",
                ChildCategories = new List<Category>(),
                IsRoot = false
            };
            _context.Categories.Add(firstCategory);
            _context.SaveChanges();
            var service = new CategoryService(_context, _options);

            var tuple = new CategoryTuple { Child = "V", Parent = "X" };
            service.AddUpdateCategories(new List<CategoryTuple> { tuple });

            Assert.False(service.CategoryDict.ContainsKey("X"));

        }

        [Fact]
        public void AddingParentToExistingChildRoot()
        {
            var firstCategory = new Category
            {
                Name = "W",
                ChildCategories = new List<Category>(),
                IsRoot = true
            };
            _context.Categories.Add(firstCategory);
            _context.SaveChanges();
            var service = new CategoryService(_context, _options);

            var tuple = new CategoryTuple { Child = "W", Parent = "X" };
            service.AddUpdateCategories(new List<CategoryTuple> { tuple });

            Assert.True(service.CategoryDict["X"] != null);
            Assert.True(service.CategoryDict["X"].ChildCategories.Count() > 0);
            Assert.True(service.CategoryDict["X"].ChildCategories[0] == service.CategoryDict["W"]);

        }

        [Fact]
        public void AddingParentToExistingChildRootWithSize1()
        {
            var firstCategory = new Category
            {
                Name = "L",
                ChildCategories = new List<Category>(),
                IsRoot = true
            };
            _context.Categories.Add(firstCategory);
            _context.SaveChanges();
            _options = Options.Create(new Constants { Depth = 1 }); ;

            var service = new CategoryService(_context, _options);

            var tuple = new CategoryTuple { Child = "L", Parent = "B" };
            service.AddUpdateCategories(new List<CategoryTuple> { tuple });

            Assert.False(service.CategoryDict.ContainsKey("B"));
        }

        [Fact]
        public void AddingParentToExistingChildRootButAncestor()
        {
            var child1 = new Category
            {
                Name = "Q",
                ChildCategories = new List<Category>(),
            };
            var child2 = new Category
            {
                Name = "E",
                ChildCategories = new List<Category> { child1 },
            };
            var firstCategory = new Category
            {
                Name = "R",
                ChildCategories = new List<Category> { child2 },
                IsRoot = true
            };
            _context.Categories.Add(firstCategory);
            _context.Categories.Add(child1);
            _context.Categories.Add(child2);
            _context.SaveChanges();
            var service = new CategoryService(_context, _options);

            var tuple = new CategoryTuple { Child = "R", Parent = "Q" };
            service.AddUpdateCategories(new List<CategoryTuple> { tuple });

            Assert.True(service.CategoryDict["Q"].ChildCategories.Count() == 0);

        }
    }
}