using Suburb.Core;
using System;
using System.Collections.Generic;
using UniRx;

namespace Suburb.Common
{
    public class MouseGestureProvider
    {
        private readonly PointerService pointerService;

        public ReactiveCommand OnDragStart { get; } = new();
        public ReactiveCommand OnDrag { get; } = new();
        public ReactiveCommand OnDragEnd { get; } = new();

        public MouseGestureProvider(PointerService pointerService)
        {
            this.pointerService = pointerService;
        }
    }
}
