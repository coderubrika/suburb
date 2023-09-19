using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suburb.Router
{
    public class RouterService
    {
        private readonly Stack<Endpoint> history = new();
        private readonly Dictionary<string, Endpoint> endpoints = new();

        public bool GoTo(string name)
        {
            if (!endpoints.TryGetValue(name, out Endpoint endpoint))
                return false;

            if (history.TryPeek(out Endpoint peekEndpoint) && peekEndpoint == endpoint)
                return false;

            history.Push(endpoint);
            return true;
        }

        public bool GoBack()
        {
            if (history.Count < 2)
                return false;

            history.Pop();
            return true;
        }

        public bool GoBackTo(string name)
        {
            if (history.Count < 2)
                return false;

            if (!endpoints.TryGetValue(name, out Endpoint endpoint))
                return false;

            if (history.Peek() == endpoint)
                return false;

            if (!history.Contains(endpoint))
            {
                history.Clear();
                history.Push(endpoint);
                return true;
            }

            history.Pop();

            while (history.TryPop(out Endpoint variant))
            {
                if (variant == endpoint)
                {
                    history.Push(endpoint);
                    break;
                }
            }

            return true;
        }

        public void AddEndpoint(Endpoint endpoint)
        {
            endpoints[endpoint.Name] = endpoint;
        }

        public bool ContainsEndpoint(string name)
        {
            return endpoints.ContainsKey(name);
        }

        public IEnumerable<string> GetHistory()
        {
            return history
                .Select(endpoint => endpoint.Name)
                .Reverse();
        }
    }
}
