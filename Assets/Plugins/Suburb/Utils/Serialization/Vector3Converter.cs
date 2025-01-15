using Newtonsoft.Json;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;

namespace Suburb.Utils.Serialization
{
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 ReadJson(
            JsonReader reader, 
            Type objectType, 
            Vector3 existingValue, 
            bool hasExistingValue, 
            JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            
            float x = jsonObject["x"]?.Value<float>() ?? 0;
            float y = jsonObject["y"]?.Value<float>() ?? 0;
            float z = jsonObject["z"]?.Value<float>() ?? 0;

            return new Vector3(x, y, z);
        }

        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("z");
            writer.WriteValue(value.z);
            writer.WriteEndObject();
        }
    }
}