using Cadastre.Data.Enumerations;
using Cadastre.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor.ImportDtos
{
    [XmlRoot("Districts")]
    public class DistrictImportDtoWrapper
    {

        [XmlElement("District")]
        public List<DistrictImportDto> Districts { get; set; } = new();
    }

    public  class DistrictImportDto
    {
        [XmlAttribute("Region")]
        public string Region { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("PostalCode")]
        public string PostalCode { get; set; }

        [XmlArray("Properties")]
        [XmlArrayItem("Property")]
        public List<PropertyImportDto> Properties { get; set; } = new();
    }

    public  class PropertyImportDto
    {
        [XmlElement("PropertyIdentifier")]
        public string PropertyIdentifier { get; set; }

        [XmlElement("Area")]
        public int Area { get; set; }

        [XmlElement("Details")]
        public string Details { get; set; }

        [XmlElement("Address")]
        public string Address { get; set; }

        [XmlElement("DateOfAcquisition")]
        public string DateOfAcquisition { get; set; }  // parse later to DateTime
    }
}
