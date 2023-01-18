using System.ComponentModel.DataAnnotations;
using WatchDb.DataAccess.Models;

namespace WatchUILibrary.Models
{
    public class Watch
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        public Category? Category { get; set; }

        public Producer? Producer { get; set; }

        public int Available { get; set; }

        public bool OnSale { get; set; }

        public byte[]? Image { get; set; }

        public Watch()
        {
        }

        public Watch(WatchModel model)
        {
            Id = model.Id;
            Title = model.Title;
            Price = model.Price;
            Available = model.Available;
            OnSale = model.OnSale;
            Image = model.Image;

            if (model.Category != null)
            {
                Category = new Category(model.Category);
            }

            if (model.Producer != null)
            {
                Producer = new Producer(model.Producer);
            }
        }
    }
}
