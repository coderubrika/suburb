using UniRx;

namespace Suburb.Common
{
    public interface IGestureProvider
    {
        public ReactiveCommand<GuestureEventData> OnPointerDown { get; }
        public ReactiveCommand<GuestureEventData> OnPointerUp { get; }
        public ReactiveCommand<GuestureEventData> OnDragStart { get; }
        public ReactiveCommand<GuestureEventData> OnDrag { get; }
        public ReactiveCommand<GuestureEventData> OnDragEnd { get; }
    }
}
