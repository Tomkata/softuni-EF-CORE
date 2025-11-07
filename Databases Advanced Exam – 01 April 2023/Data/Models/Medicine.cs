using Medicines.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.Data.Models
{
    public class Medicine
    {
        [Key]
        public int Id { get; set; }
        [StringLength(150, MinimumLength = 3)]
        [Required]
        public string Name { get; set; } = null!;
        [Range(0.01, 1000.00)]
        [Required]
        public decimal Price { get; set; }
        [Required]
        public Category Category { get; set; }

        [Required]
        public DateTime ProductionDate  { get; set; }
        [Required]
        public DateTime   ExpiryDate  { get; set; }
        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string Producer { get; set; } = null!;
        [ForeignKey(nameof(Pharmacy))]
        public int 	PharmacyId  { get; set; }
        public Pharmacy? Pharmacy { get; set; }

        public virtual ICollection<PatientMedicine> PatientsMedicines { get; set; } = new HashSet<PatientMedicine>();

    }
}       
