using Cadastre.Data.Enumerations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Property = Cadastre.Data.Models.Property;

namespace Cadastre.Data.Models
{
    public class District
    {
        [Key]
        public int Id { get; set; }
        [StringLength(80, MinimumLength = 2)]
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(8)]
        [RegularExpression("^[A-Z]{2}-[0-9]{5}$")]
        public string PostalCode { get; set; } = null!;
        public Region Region { get; set; }
        public virtual ICollection<Property> Properties { get; set; } = new HashSet<Property>();

    }
}
