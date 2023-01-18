using System.ComponentModel.DataAnnotations;
using WatchDb.DataAccess.Models;

namespace WatchUILibrary.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = null!;

        public Category()
        {
        }

        public Category(CategoryModel model)
        {
            Id = model.Id;
            CategoryName = model.CategoryName;
        }
    }
}
