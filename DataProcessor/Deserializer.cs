using Cadastre.Data;
using Cadastre.Data.Enumerations;
using Cadastre.Data.Models;
using Cadastre.DataProcessor.ImportDtos;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor
{
    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid Data!";
        private const string SuccessfullyImportedDistrict =
            "Successfully imported district - {0} with {1} properties.";
        private const string SuccessfullyImportedCitizen =
            "Succefully imported citizen - {0} {1} with {2} properties.";

        public static string ImportDistricts(CadastreContext dbContext, string xmlDocument)
        {
            var sb = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(DistrictImportDtoWrapper));

            using var reader = new StringReader(xmlDocument);
            var dtoWrapper = (DistrictImportDtoWrapper)serializer.Deserialize(reader);

            var districtDic = new Dictionary<string, District>();
            var propertiesDic = new Dictionary<string, Property>();

            foreach (var dto in dtoWrapper.Districts)
            {
                if (string.IsNullOrWhiteSpace(dto.Name) ||
                    string.IsNullOrWhiteSpace(dto.PostalCode) ||
                    dto.Name.Length < 2 || dto.Name.Length > 80 ||
                    !System.Text.RegularExpressions.Regex.IsMatch(dto.PostalCode, @"^[A-Z]{2}-\d{5}$"))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!Enum.TryParse(dto.Region, out Region region))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (districtDic.ContainsKey(dto.Name) || dbContext.Districts.Any(d => d.Name == dto.Name))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var district = new District
                {
                    Name = dto.Name,
                    PostalCode = dto.PostalCode,
                    Region = region,
                    Properties = new List<Property>()
                };

                foreach (var propertyDto in dto.Properties)
                {
                    if (string.IsNullOrWhiteSpace(propertyDto.PropertyIdentifier) ||
                        propertyDto.PropertyIdentifier.Length < 16 || propertyDto.PropertyIdentifier.Length > 20 ||
                        string.IsNullOrWhiteSpace(propertyDto.Details) ||
                        string.IsNullOrWhiteSpace(propertyDto.Address))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (propertiesDic.ContainsKey(propertyDto.PropertyIdentifier) ||
                        district.Properties.Any(p => p.PropertyIdentifier == propertyDto.PropertyIdentifier) ||
                        district.Properties.Any(p => p.Address == propertyDto.Address) ||
                        dbContext.Properties.Any(p => p.PropertyIdentifier == propertyDto.PropertyIdentifier) ||
                        dbContext.Properties.Any(p => p.Address == propertyDto.Address))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var property = new Property
                    {
                        PropertyIdentifier = propertyDto.PropertyIdentifier,
                        Address = propertyDto.Address,
                        Area = propertyDto.Area,
                        DateOfAcquisition = DateTime.ParseExact(propertyDto.DateOfAcquisition, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Details = propertyDto.Details
                    };

                    district.Properties.Add(property);
                    propertiesDic[property.PropertyIdentifier] = property;
                }

                sb.AppendLine(string.Format(SuccessfullyImportedDistrict, district.Name, district.Properties.Count));
                districtDic[dto.Name] = district;
            }

            dbContext.AddRange(districtDic.Values);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        public static string ImportCitizens(CadastreContext dbContext, string jsonDocument)
        {
            var sb = new StringBuilder();
            var citizens = JsonConvert.DeserializeObject<ICollection<CitizenImportDto>>(jsonDocument);

            var validStatuses = new[] { "Unmarried", "Married", "Divorced", "Widowed" };
            var citizensToAdd = new List<Citizen>();

            var propertiesDict = dbContext.Properties
                .AsNoTracking()
                .ToDictionary(p => p.Id);

            foreach (var dto in citizens)
            {
                if (string.IsNullOrWhiteSpace(dto.FirstName) ||
                    string.IsNullOrWhiteSpace(dto.LastName) ||
                    dto.FirstName.Length < 2 || dto.FirstName.Length > 30 ||
                    dto.LastName.Length < 2 || dto.LastName.Length > 30 ||
                    !validStatuses.Contains(dto.MaritalStatus))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!DateTime.TryParseExact(dto.BirthDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthDate))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!Enum.TryParse(dto.MaritalStatus, true, out MaritalStatus maritalStatus))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var citizen = new Citizen
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    MaritalStatus = maritalStatus,
                    BirthDate = birthDate,
                    PropertiesCitizens = new List<PropertyCitizen>()
                };

                foreach (var propId in dto.Properties.Distinct())
                {
                    if (propertiesDict.ContainsKey(propId))
                    {
                        citizen.PropertiesCitizens.Add(new PropertyCitizen
                        {
                            PropertyId = propId
                        });
                    }
                }

                citizensToAdd.Add(citizen);
                sb.AppendLine(string.Format(
                    SuccessfullyImportedCitizen,
                    citizen.FirstName,
                    citizen.LastName,
                    citizen.PropertiesCitizens.Count));
            }

            dbContext.Citizens.AddRange(citizensToAdd);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}