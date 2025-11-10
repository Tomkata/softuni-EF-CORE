using System.Xml;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ExportDto
{
    public class ExportUserPurchaseGameDto
    {
        public ExportUserPurchaseGameDto()
        {
        }

        [XmlAttribute("title")]
        public string Name { get; set; }
        [XmlElement("Genre")]
        public string Genre { get; set; }
        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}