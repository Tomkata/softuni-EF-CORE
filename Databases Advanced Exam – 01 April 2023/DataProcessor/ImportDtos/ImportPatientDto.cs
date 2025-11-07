using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.DataProcessor.ImportDtos
{
    public class ImportPatientDto
    {
        public string FullName { get; set; }
        public int AgeGroup { get; set; }
        public int Gender { get; set; }

        public List<int> Medicines { get; set; } = new();
    }
}
