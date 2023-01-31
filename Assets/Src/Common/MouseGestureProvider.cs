using Suburb.Core;
using System;
using System.Collections.Generic;
using UniRx;

namespace Suburb.Common
{
    public class MouseGestureProvider : IGestureProvider
    {
        public ReactiveCommand<GuestureEventData> OnPointerDown => throw new NotImplementedException();

        public ReactiveCommand<GuestureEventData> OnPointerUp => throw new NotImplementedException();

        public ReactiveCommand<GuestureEventData> OnDragStart => throw new NotImplementedException();

        public ReactiveCommand<GuestureEventData> OnDrag => throw new NotImplementedException();

        public ReactiveCommand<GuestureEventData> OnDragEnd => throw new NotImplementedException();
    }
}
