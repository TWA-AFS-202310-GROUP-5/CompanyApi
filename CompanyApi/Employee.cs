namespace CompanyApi
{
    public class Employee
    {
        public Employee(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public override bool Equals(object? obj)
        {
            Employee? employee = obj as Employee;
            return Id.Equals(employee?.Id) && Name.Equals(employee?.Name);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
