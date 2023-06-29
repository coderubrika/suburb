using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Suburb.Utils
{
    public class UIUtils
    {
        public static IDisposable UpdateContent(RectTransform content)
        {
            IDisposable disposable = null;
            disposable = Observable.NextFrame()
                .Subscribe(_ =>
                {
                    disposable.Dispose();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                });

            return disposable;
        }

        public static IDisposable UpdateContents(IEnumerable<RectTransform> contents)
        {
            IDisposable disposable = null;

            disposable = Observable.NextFrame()
                .Subscribe(_ =>
                {
                    disposable.Dispose();
                    foreach (var content in contents)
                        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                });

            return disposable;
        }
    }
}
