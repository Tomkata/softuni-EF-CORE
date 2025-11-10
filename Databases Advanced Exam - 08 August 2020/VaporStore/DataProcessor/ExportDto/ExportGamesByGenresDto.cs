using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaporStore.DataProcessor.ExportDto
{
    public class ExportGamesByGenresDto
    {
        public int Id { get; set; }
        public string Genre { get; set; }
        public List<GamesDtoExport> Games { get; set; } = new();
    }
}
