using System;

namespace Suburb.UI
{
    public interface IUIAnimation
    {
        public IDisposable Animate();
        public bool CheckAllow();
    }
}