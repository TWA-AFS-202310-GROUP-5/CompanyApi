namespace CompanyApi
{
    public class EmployeeRequest
    {
        public EmployeeRequest(double salary, string name)
        {
            Salary = salary;
            Name = name;
        }

        public string Name { get; set; }
        public double Salary { get; set; }
    }
}
