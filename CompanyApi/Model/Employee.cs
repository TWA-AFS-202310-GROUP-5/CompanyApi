namespace CompanyApi
{
    public class Employee
    {
        public Employee(string name, double salary)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Salary = salary;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }
    }
}
