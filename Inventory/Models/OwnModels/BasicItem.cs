namespace Inventory.Models.OwnModels
{
    public class BasicItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] OrderedName { get; set; }
        public decimal Price { get; set; }
        public decimal PriceUnit { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string Company { get; set; }
        public byte[]? Image { get; set; }
        public string? ImageUrl { get; set; }
        public BasicItem() 
        {
            Id = 0;
        }
    }
}
