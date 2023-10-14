using System;
using Suburb.ExpressRouter;

namespace Suburb.UI
{
    public interface IUIAnimation
    {
        public MiddlewareOrder Order { get; }
        public IDisposable Animate(Action next);
        public bool CheckAllow();
    }
}