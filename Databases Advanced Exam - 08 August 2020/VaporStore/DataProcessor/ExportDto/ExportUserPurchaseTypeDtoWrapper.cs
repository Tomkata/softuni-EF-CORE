using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ExportDto
{
    [XmlType("User")]
    public class ExportUserPurchaseTypeDtoWrapper
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlArray("Purchases")]
        public ExportUserPurchaseDto[] Purchases { get; set; } = Array.Empty<ExportUserPurchaseDto>();
        [XmlElement("TotalSpent")]
        public decimal TotalSpent { get; set; }

    }
}
