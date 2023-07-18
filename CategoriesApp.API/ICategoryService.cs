using CategoriesApp.API.Entities;

namespace CategoriesApp.API
{
    public interface ICategoryService
    {
        void AddUpdateCategories(List<CategoryTuple> catTuples);
        List<Category> GetCategories();
    }
}