namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ImportDtos;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            var patients = JsonConvert.DeserializeObject<ImportPatientDto[]>(jsonString);

            var sb = new StringBuilder();
            var patientsToAdd = new HashSet<Patient>();

            foreach (var patientDto in patients)
            {
                if (string.IsNullOrWhiteSpace(patientDto.FullName)
                   || patientDto.FullName.Length < 5
                   || patientDto.FullName.Length > 100
                   || !Enum.IsDefined(typeof(AgeGroup), patientDto.AgeGroup)
                   || !Enum.IsDefined(typeof(Gender), patientDto.Gender))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Patient patient = new Patient   
                {
                    FullName = patientDto.FullName,
                    AgeGroup = (AgeGroup)patientDto.AgeGroup,
                    Gender = (Gender)patientDto.Gender
                };

                var addedMedicineIds = new HashSet<int>();


                foreach (var medicineId in patientDto.Medicines)
                {
                    var medicine = context.Medicines.Find(medicineId);

                    if (medicine == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (addedMedicineIds.Contains(medicineId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    patient.PatientsMedicines.Add(new PatientMedicine
                    {
                        Medicine = medicine,
                        Patient = patient
                    });

                    addedMedicineIds.Add(medicineId);

                }

                patientsToAdd.Add(patient);
                sb.AppendLine(string.Format(SuccessfullyImportedPatient, patient.FullName, patient.PatientsMedicines.Count));
            }

            context.Patients.AddRange(patientsToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }


        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportPharmaciesRootDto));
            using var reader = new StringReader(xmlString);
            var root = (ImportPharmaciesRootDto)xmlSerializer.Deserialize(reader)!;

            var sb = new StringBuilder();
            var pharmacies = new List<Pharmacy>();

            foreach (var dto in root.Pharmacies)
            {
                bool isNonStopValid = dto.NonStop?.ToLower() == "true" || dto.NonStop?.ToLower() == "false";
                bool nonStopValue = dto.NonStop?.ToLower() == "true";

                if (string.IsNullOrWhiteSpace(dto.Name)
                    || dto.Name.Length < 3 || dto.Name.Length > 50
                    || string.IsNullOrWhiteSpace(dto.PhoneNumber)
                    || !isNonStopValid
                    || !Regex.IsMatch(dto.PhoneNumber.Trim(), @"^\([0-9]{3}\) [0-9]{3}-[0-9]{4}$"))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var seen = new HashSet<(string Name, string Producer)>();
                var validMeds = new List<Medicine>();

                foreach (var med in dto.Medicines)
                {
                    DateTime productionDate = default;
                    DateTime expiryDate = default;

                    bool invalid =
                        string.IsNullOrWhiteSpace(med.Name) ||
                        med.Name.Length < 3 || med.Name.Length > 150 ||
                        string.IsNullOrWhiteSpace(med.Producer) ||
                        med.Producer.Length < 3 || med.Producer.Length > 100 ||
                        med.Price < 0.01m || med.Price > 1000.00m ||
                        !DateTime.TryParseExact(med.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out productionDate) ||
                        !DateTime.TryParseExact(med.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out expiryDate) ||
                        productionDate >= expiryDate ||
                        !Enum.IsDefined(typeof(Category), med.Category);

                    if (invalid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var key = (med.Name.Trim(), med.Producer.Trim());
                    if (!seen.Add(key))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    validMeds.Add(new Medicine
                    {
                        Name = med.Name.Trim(),
                        Price = med.Price,
                        Category = (Category)med.Category,
                        ProductionDate = productionDate,
                        ExpiryDate = expiryDate,
                        Producer = med.Producer.Trim()
                    });
                }

                // Авторското решение импортира аптеки ДОРИ с 0 medicines!

                var pharmacy = new Pharmacy
                {
                    Name = dto.Name.Trim(),
                    PhoneNumber = dto.PhoneNumber.Trim(),
                    IsNonStop = nonStopValue,
                    Medicines = validMeds
                };

                pharmacies.Add(pharmacy);
                sb.AppendLine(string.Format(SuccessfullyImportedPharmacy, pharmacy.Name, validMeds.Count));
            }

            context.Pharmacies.AddRange(pharmacies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
    }
}
