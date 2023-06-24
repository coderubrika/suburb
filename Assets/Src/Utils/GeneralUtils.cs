using System;
using UniRx;

namespace Suburb.Utils
{
    public static class GeneralUtils
    {
        public static IObservable<T> StartWithDefault<T> (T defaultValue = default)
        {
            return Observable.Start(() => defaultValue);
        }
    }
}
