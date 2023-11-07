namespace CompanyApi
{
    public class Employee
    {
        public Employee(string name, int salary)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Salary = salary;
        }

        public string Id { get; set; }

        public string Name { get; set; }
        public int Salary { get; set; }

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
