using System;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Suburb.Utils.Serialization
{
    public class EaseConverter : JsonConverter<Ease>
    {
        public override void WriteJson(JsonWriter writer, Ease value, JsonSerializer serializer)
        {
            writer.WritePropertyName("easing");
            writer.WriteValue((int)value);
        }

        public override Ease ReadJson(JsonReader reader, Type objectType, Ease existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            int tryEasing = jsonObject["easing"]?.Value<int>() ?? int.MaxValue;

            return tryEasing == int.MaxValue ? hasExistingValue ? existingValue : Ease.Linear : (Ease)tryEasing;
        }
    }
}