using Cadastre.Data;
using Cadastre.DataProcessor.ExportDtos;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor
{
    public class Serializer
    {
        public static string ExportPropertiesWithOwners(CadastreContext dbContext)
        {
            var properties = dbContext.Properties
                .AsNoTracking()
                .Where(p => p.DateOfAcquisition >= DateTime.ParseExact("01/01/2000", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                .Select(p => new
                {
                    p.PropertyIdentifier,
                    p.Address,
                    p.Area,
                    p.DateOfAcquisition,
                    Owners = p.PropertiesCitizens
                        .Select(pc => new
                        {
                            pc.Citizen.LastName,
                            MaritalStatus = pc.Citizen.MaritalStatus.ToString()
                        })
                        .OrderBy(o => o.LastName)
                        .ToList()
                })
                .ToList()
                .Select(p => new ExportPropertiesWithOwnersDto
                {
                    PropertyIdentifier = p.PropertyIdentifier,
                    Address = p.Address,
                    Area = p.Area,
                    DateOfAcquisition = p.DateOfAcquisition.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Owners = p.Owners
                        .Select(o => new OwnerDto
                        {
                            LastName = o.LastName,
                            MaritalStatus = o.MaritalStatus
                        })
                        .ToList()
                })
                .OrderByDescending(p => DateTime.ParseExact(p.DateOfAcquisition, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                .ThenBy(p => p.PropertyIdentifier)
                .ToList();

            string json = JsonConvert.SerializeObject(properties, Formatting.Indented);
            return json.TrimEnd();
        }

        public static string ExportFilteredPropertiesWithDistrict(CadastreContext dbContext)
        {
            var properties = dbContext.Properties
                .AsNoTracking()
                .Where(p => p.Area >= 100)
                .Select(p => new PropertyDtoExport
                {
                    PropertyIdentifier = p.PropertyIdentifier,
                    Area = p.Area,
                    DateOfAcquisition = p.DateOfAcquisition.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    PostalCode = p.District.PostalCode
                })
                .OrderByDescending(p => p.Area)
                .ThenBy(p => DateTime.ParseExact(p.DateOfAcquisition, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                .ToArray();

            var serializer = new XmlSerializer(typeof(PropertyDtoExport[]), new XmlRootAttribute("Properties"));
            var sb = new StringBuilder();
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, properties, ns);
            }

            return sb.ToString().TrimEnd();
        }
    }
}
