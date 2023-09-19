namespace Suburb.Router
{
    public class Endpoint : IEndpoint
    {
        public string Name { get; }

        public Endpoint(string name)
        {
            Name = name;
        }
    }
}
