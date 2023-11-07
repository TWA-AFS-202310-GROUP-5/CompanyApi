namespace CompanyApi
{
    public class Company
    {
        public Company(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public override bool Equals(object? obj)
        {
            Company? company = obj as Company;
            return Id.Equals(company?.Id) && Name.Equals(company?.Name);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
