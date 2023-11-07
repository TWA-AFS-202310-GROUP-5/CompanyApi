namespace CompanyApi
{
    public class CreateEmployeeRequest
    {
        public required string Name { get; set; }
        public int Salary { get; set; }
    }
}
