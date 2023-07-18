using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CategoriesApp.API.Entities
{
    [Index("Name", IsUnique = true)]
    public class Category
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Category> ChildCategories { get; set; } = new List<Category>();
        public bool IsRoot { get; set; }
        public int? ParentCategoryId { get; set; } // Foreign key property

        public Category ParentCategory { get; set; } // Navigation property
    }
}
