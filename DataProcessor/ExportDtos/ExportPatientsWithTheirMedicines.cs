using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlRoot("Patients")]
    public class ExportPatientsWithTheirMedicines
    {
        [XmlElement("Patient")]
        public PatientDto[] Patients { get; set; } = null!;
    }
}
