using System;
using Suburb.ExpressRouter;

namespace Suburb.UI
{
    public interface IUIAnimation
    {
        public MiddlewareOrder Order { get; }
        public IDisposable Animate((IEndpoint From, IEndpoint To) args, Action<(IEndpoint From, IEndpoint To)> next);
        public bool CheckAllow();
    }
}