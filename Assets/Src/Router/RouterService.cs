using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suburb.Router
{
    public class RouterService
    {
        private const string ALL = "*";
        private const string ALL_FILTER = "*->*";

        private readonly Stack<Endpoint> history = new();
        private readonly Dictionary<string, Endpoint> endpoints = new();
        private readonly Dictionary<string, Action<Endpoint, Endpoint>> middlewares = new();

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

        public void Use(Action<Endpoint, Endpoint> middleware, string nameFrom = null, string nameTo = null)
        {
            (nameFrom, nameTo) = TransformNames(nameFrom, nameTo);

            string key = $"{nameFrom}->{nameTo}";

            if (middlewares.ContainsKey(key))
                middlewares[key] += middleware;

            middlewares.Add($"{nameFrom}->{nameTo}", middleware);
        }

        private void ApplyMiddlewares(Endpoint from = null, Endpoint to = null)
        {
            string nameFrom, nameTo, filter;
            (nameFrom, nameTo) = TransformNames(from?.Name, to?.Name);

            if (middlewares.TryGetValue(ALL_FILTER, out Action<Endpoint, Endpoint> middleware))
                middlewares[ALL_FILTER]?.Invoke(from, to);

            filter = $"{nameFrom}->*";
            if (nameFrom != ALL && middlewares.TryGetValue(filter, out middleware))
                middlewares[filter]?.Invoke(from, to);

            filter = $"*->{nameTo}";
            if (nameTo != ALL && middlewares.TryGetValue(filter, out middleware))
                middlewares[filter]?.Invoke(from, to);

            filter = $"{nameFrom}->{nameTo}";
            if (nameFrom != ALL && nameTo != ALL && middlewares.TryGetValue(filter, out middleware))
                middlewares[filter]?.Invoke(from, to);
        }

        private (string NameFrom, string NameTo) TransformNames(string nameFrom, string nameTo)
        {
            if (string.IsNullOrEmpty(nameFrom))
                nameFrom = ALL;

            if (string.IsNullOrEmpty(nameTo))
                nameTo = ALL;

            return new ValueTuple<string, string>(nameFrom, nameTo);
        }
    }
}
