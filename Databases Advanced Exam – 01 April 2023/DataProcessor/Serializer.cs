namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ExportDtos;
    using Medicines.DataProcessor.ImportDtos;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net.WebSockets;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {

            var parsedDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var patientsRaw = context.Patients
        .AsNoTracking()
        .Where(p => p.PatientsMedicines.Any(pm => pm.Medicine.ProductionDate > parsedDate))
        .Select(p => new
        {
            p.FullName,
            p.AgeGroup,
            p.Gender,
            Medicines = p.PatientsMedicines
                .Where(pm => pm.Medicine.ProductionDate > parsedDate)
                .Select(pm => new
                {
                    pm.Medicine.Name,
                    pm.Medicine.Price,
                    pm.Medicine.Category,
                    pm.Medicine.Producer,
                    pm.Medicine.ExpiryDate
                })
                .OrderByDescending(m => m.ExpiryDate)
                .ThenBy(m => m.Price)
                .ToArray()
        })
        .ToList(); 

            var patients = patientsRaw
                .Select(p => new PatientDto
                {
                    Name = p.FullName,
                    AgeGroup = p.AgeGroup.ToString(),
                    Gender = p.Gender.ToString().ToLower(),
                    Medicines = p.Medicines
                        .Select(m => new MedicinesDto
                        {
                            Name = m.Name,
                            Category = m.Category.ToString().ToLower(),
                            Price = m.Price.ToString("0.00"),
                            Producer = m.Producer,
                            ExpiryDate = m.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                        })
                        .ToArray()
                })
                .OrderByDescending(p => p.Medicines.Length)
                .ThenBy(p => p.Name)
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportPatientsWithTheirMedicines));

            var sb = new StringBuilder();

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                serializer.Serialize(writer, new ExportPatientsWithTheirMedicines
                {
                    Patients = patients
                }, ns);
            }

            return sb.ToString().TrimEnd();


        }

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            var medicines = context.Medicines
                .AsNoTracking()
                .Where(x => (int)x.Category == medicineCategory && x.Pharmacy.IsNonStop)
                .OrderBy(x => x.Price)
                .ThenByDescending(x => x.Name)
                .Select(x => new ExportMedicinesDto
                {
                    Name = x.Name,
                    Price = x.Price.ToString("0.00"),
                    Pharmacy = new ExportPharmacyDto
                    {
                        Name = x.Pharmacy.Name,
                        PhoneNumber = x.Pharmacy.PhoneNumber
                    }
                })
               .ToList();

            return JsonConvert.SerializeObject(medicines, Newtonsoft.Json.Formatting.Indented);


        }
    }
}
