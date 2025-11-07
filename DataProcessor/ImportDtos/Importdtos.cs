using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
  
    [XmlRoot("Pharmacies")]
    public class ImportPharmaciesRootDto
    {
        [XmlElement("Pharmacy")]
        public ImportPharmacyDto[] Pharmacies { get; set; } = null!;
    }

    public class ImportPharmacyDto
    {
        [XmlAttribute("non-stop")]
        public string? NonStop { get; set; }

        [XmlElement("Name")] 
        public string Name { get; set; } = null!;

        [XmlElement("PhoneNumber")]
        public string PhoneNumber { get; set; } = null!;

        [XmlArray("Medicines")]
        [XmlArrayItem("Medicine")]
        public ImportMedicineDto[] Medicines { get; set; } = Array.Empty<ImportMedicineDto>();
    }

    public class ImportMedicineDto
    {
        [XmlAttribute("category")]
        public int Category { get; set; }

        [XmlElement("Name")] 
        public string Name { get; set; } = null!;

        [XmlElement("Price")]
        public decimal Price { get; set; }

        [XmlElement("ProductionDate")]
        public string ProductionDate { get; set; } = null!;

        [XmlElement("ExpiryDate")]
        public string ExpiryDate { get; set; } = null!;

        [XmlElement("Producer")]
        public string Producer { get; set; } = null!;
    }
}