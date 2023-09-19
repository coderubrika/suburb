using Suburb.Utils;
using System.Collections;
using System.Text;
using UnityEngine;

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

    public class RouterTest : MonoBehaviour
    {
        private readonly RouterService routerService = new RouterService();

        private void Awake()
        {
            AddAndGoTo(5);
            AddAndGoTo("4");
            GoBack(5);
            AddAndGoTo(5);
            GoBackTo("2");
            GoBackTo("3");
            GoBackTo("3");
            GoBackTo("f");
        }

        private void AddAndGoTo(string name)
        {
            if (!routerService.ContainsEndpoint(name))
                routerService.AddEndpoint(new Endpoint(name));

            routerService.GoTo(name);
            this.Log(string.Join('/', routerService.GetHistory()));
        }

        private void AddAndGoTo(int count)
        {
            for (int i = 0; i < count; i++)
                AddAndGoTo(i.ToString());
        }

        private void GoBack(int count)
        {
            for (int i = 0; i < count; i++)
            {
                routerService.GoToPrevious();
                this.Log(string.Join('/', routerService.GetHistory()));
            }
        }

        private void GoBackTo(string name)
        {
            routerService.GoToPrevious(name);
            this.Log(string.Join('/', routerService.GetHistory()));
        }
    }
}