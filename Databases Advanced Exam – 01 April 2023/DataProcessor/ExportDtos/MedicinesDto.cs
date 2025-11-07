using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    public class MedicinesDto
    {
        [XmlAttribute("Category")]
        public string Category { get; set; } = null!;
        [XmlElement("Name")]
        public string Name { get; set; } = null!;
        [XmlElement("Price")]
        public string Price { get; set; }
        [XmlElement("Producer")]
        public string Producer { get; set; } = null!;
        [XmlElement("BestBefore")]
        public string ExpiryDate { get; set; } = null!;
    }
}