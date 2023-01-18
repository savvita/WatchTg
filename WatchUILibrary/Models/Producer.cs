using System.ComponentModel.DataAnnotations;
using WatchDb.DataAccess.Models;

namespace WatchUILibrary.Models
{
    public class Producer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ProducerName { get; set; } = null!;

        public Producer()
        {
        }

        public Producer(ProducerModel model)
        {
            Id = model.Id;
            ProducerName = model.ProducerName;
        }
    }
}
