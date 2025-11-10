using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ImportDto
{
    [XmlRoot("Purchases")]
    public class PurshaseImportWrapperDto
    {
        [XmlElement("Purchase")]
        public PurshaseDto[] Purshases { get; set; } = Array.Empty<PurshaseDto>();

    }
}
