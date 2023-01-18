using System.ComponentModel.DataAnnotations;
using WatchDb.DataAccess.Models;

namespace WatchUILibrary.Models
{
    public class Status
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string StatusName { get; set; } = null!;

        public Status()
        {
        }

        public Status(StatusModel model)
        {
            Id = model.Id;
            StatusName = model.StatusName;
        }
    }
}
