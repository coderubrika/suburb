using Suburb.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suburb.Loaders
{
    public class ResourceMap : IInstallable
    {
        IInstallable[] installables;

        public void Install()
        {
            foreach (var installable in installables)
            {
                installable.Install();
            }
        }

        public void Uninstall()
        {
            foreach (var installable in installables)
            {
                installable.Uninstall();
            }
        }
    }
}
