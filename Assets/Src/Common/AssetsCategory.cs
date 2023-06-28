using System;
using Newtonsoft.Json;

namespace Suburb.Common
{
    [Serializable]
    public class AssetsCategory<T>
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("assets_list")]
        public T[] AssetsList;
    }
}
