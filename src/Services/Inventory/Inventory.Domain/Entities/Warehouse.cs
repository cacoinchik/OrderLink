namespace Inventory.Domain.Entities
{
    public class Warehouse
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Region { get; private set; }
        public string City { get; private set; }
        public string Address { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime TimeCreate { get; private set; }

        private Warehouse() { }

        public Warehouse(string name, string region, string city, string address)
        {
            Id = Guid.NewGuid();
            Name = name;
            Region = region;
            City = city;
            Address = address;
            IsActive = true;
            TimeCreate = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}