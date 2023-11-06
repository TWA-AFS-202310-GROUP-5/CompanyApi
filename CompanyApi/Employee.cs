namespace CompanyApi
{
    public class Employee
    {


        public string Name { get; set; }
        public string Id { get; set; }



        public Employee(string name)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
        }
    }
}
