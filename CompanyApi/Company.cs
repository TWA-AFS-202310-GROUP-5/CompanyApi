namespace CompanyApi
{
    public class Company
    {
        private readonly Dictionary<string, Employee> employeeMap = new Dictionary<string, Employee>(); 
        public Company(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public Employee AddEmployee(CreateEmployeeRequest request)
        {
            Employee emp = new Employee(request.Name);
            employeeMap.Add(emp.Id, emp);
            return emp;
        }
    }
}
