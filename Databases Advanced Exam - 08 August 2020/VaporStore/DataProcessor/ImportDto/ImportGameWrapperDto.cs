using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.ImportDto
{
    public class ImportGameWrapperDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ReleaseDate { get; set; }
        public string Developer { get; set; }
        public string Genre { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
