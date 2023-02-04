using System;
using System.Collections.Generic;
using UniRx;

namespace Suburb.Core.Inputs
{
    public class TouchGestureProvider : IGestureProvider
    {
        public ReactiveCommand<GestureEventData> OnPointerDown => throw new NotImplementedException();

        public ReactiveCommand<GestureEventData> OnPointerUp => throw new NotImplementedException();

        public ReactiveCommand<GestureEventData> OnDragStart => throw new NotImplementedException();

        public ReactiveCommand<GestureEventData> OnDrag => throw new NotImplementedException();

        public ReactiveCommand<GestureEventData> OnDragEnd => throw new NotImplementedException();

        public ReactiveCommand<GestureEventData> OnZoom => throw new NotImplementedException();

        public bool IsDragging => throw new NotImplementedException();
    }
}
