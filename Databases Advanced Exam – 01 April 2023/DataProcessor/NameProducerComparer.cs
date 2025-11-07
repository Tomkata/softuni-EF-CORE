
using System.Diagnostics.CodeAnalysis;

namespace Medicines.DataProcessor
{
    internal class NameProducerComparer : IEqualityComparer<(string Name, string Producer)>
    {
        public bool Equals((string Name, string Producer) x, (string Name, string Producer) y)
        {
            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                 && string.Equals(x.Producer, y.Producer, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] (string Name, string Producer) obj)
        {
            int hashName = obj.Name?.ToLowerInvariant().GetHashCode() ?? 0;
            int hashProducer = obj.Producer?.ToLowerInvariant().GetHashCode() ?? 0;
            return hashName ^ hashProducer;
        }
    }
}