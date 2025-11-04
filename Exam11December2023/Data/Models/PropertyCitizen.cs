using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastre.Data.Models
{
    public class PropertyCitizen
    {
        [Key]
        public int CitizenId { get; set; }
        public Citizen Citizen { get; set; }

        [Key]
        public int PropertyId { get; set; }
        public Property Property { get; set; }

    }
}
