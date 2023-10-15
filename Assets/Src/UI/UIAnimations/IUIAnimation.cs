using System;
using Suburb.ExpressRouter;

namespace Suburb.UI
{
    public interface IUIAnimation
    {
        public ActItem<FromTo> Animate { get; }
    }
}