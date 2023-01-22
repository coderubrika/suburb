
using UnityEngine;

namespace Suburb.Loaders
{
    public class ResourceLoader
    {
        private ResourceMap resource;

        public void LoadResource(ResourceMap resourceMap)
        {
            if (resource != null)
            {
                Debug.LogError("previous resource not freed");
                return;
            }

            resource = resourceMap;
            resource.Install();
        }

        public void FreeResource()
        {
            resource.Uninstall();
            resource = null;
        }
    }
}
