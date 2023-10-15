using System;
using Suburb.ExpressRouter;

namespace Suburb.UI
{
    public interface IUIAnimation
    {
        public MiddlewareOrder Order { get; }
        public ActItem<FromTo> Animate { get; }
    }
}