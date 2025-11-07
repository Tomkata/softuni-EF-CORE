using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.Data.Models
{
    public class Pharmacy
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 2)]
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        [RegularExpression(@"^\([0-9]{3}\) [0-9]{3}-[0-9]{4}$")]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public bool IsNonStop { get; set; }
        public virtual ICollection<Medicine> Medicines { get; set; } = new HashSet<Medicine>();
    }
}
