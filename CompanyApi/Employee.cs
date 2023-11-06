namespace CompanyApi
{
    public class Employee
    {

        public string CompanyId { get; set; }

        public string Name { get; set; }
        public string Id { get; set; }

        public Employee()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Employee(string companyId, string name)
        {
            CompanyId = companyId;
            Name = name;
            Id = Guid.NewGuid().ToString();
        }
    }
}
