using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cadastre.DataProcessor.ExportDtos
{
    public class ExportPropertiesWithOwnersDto
    {
        public string PropertyIdentifier { get; set; }
        public int Area { get; set; }
        public string Address { get; set; }
        public string DateOfAcquisition { get; set; }
        public ICollection<OwnerDto> Owners { get; set; } = new List<OwnerDto>();

    }
}
