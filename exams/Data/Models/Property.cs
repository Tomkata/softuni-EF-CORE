using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastre.Data.Models
{
    public class Property
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(20,MinimumLength =16)]
        public string PropertyIdentifier { get; set; } = null!;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Area cannot be negative.")]
        public int Area { get; set; }

        [StringLength(500, MinimumLength = 5)]

        public string? Details { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Address { get; set; } = null!;

        [Required]
        public DateTime DateOfAcquisition { get; set; }

        [ForeignKey(nameof(District))]
        public int DistrictId { get; set; }
        public District District { get; set; } = null!;

        public ICollection<PropertyCitizen> PropertiesCitizens { get; set; }  = new HashSet<PropertyCitizen>();

    }
}
