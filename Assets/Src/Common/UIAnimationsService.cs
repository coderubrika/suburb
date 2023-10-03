using System.Collections.Generic;
using System;
using Suburb.Screens;
using Suburb.UI;
using Suburb.UI.Screens;
using UnityEngine;

namespace Suburb.Common
{
    public class UIAnimationsService
    {
        private readonly ScreensService screensService;
        private readonly Dictionary<Type, IResourceMap> resourceMaps = new();

        private IDisposable currentAnimation;
        
        public UIAnimationsService(ScreensService screensService)
        {
            this.screensService = screensService;
        }
        
        public void AddResourceMap<TMap>(TMap map)
            where TMap : IResourceMap
        {
            Type type = typeof(TMap);
            if (resourceMaps.ContainsKey(type))
                return;
            
            resourceMaps.Add(type, map);
        }

        public T GetResourceMap<T>()
            where T : IResourceMap
        {
            Type type = typeof(T);
            return resourceMaps.TryGetValue(type, out IResourceMap map) && map is T tMap
                ? tMap : default;
        }

        public void AddAnimation<TFrom, TTo>(IUIAnimation uiAnimation)
            where TFrom : BaseScreen
            where TTo : BaseScreen
        {
            screensService.UseTransition<TFrom, TTo>((from, to) =>
            {
                if (!uiAnimation.CheckAllow())
                    return;
                
                currentAnimation?.Dispose();
                currentAnimation = uiAnimation.Animate();
            });
        }
    }
}