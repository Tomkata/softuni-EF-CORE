namespace VaporStore.DataProcessor.ExportDto
{
    internal class ExportUserDto
    {
        public ExportUserDto()
        {
        }

        public string Username { get; set; }
        public object Purchases { get; set; }
        public decimal TotalSpent { get; set; }
    }
}