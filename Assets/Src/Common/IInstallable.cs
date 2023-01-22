using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suburb.Common
{
    public interface IInstallable
    {
        public void Install();
        public void Uninstall();
    }
}
