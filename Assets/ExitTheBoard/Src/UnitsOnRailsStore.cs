using System.Collections.Generic;
using System.Linq;
using Suburb.Utils;

namespace ExitTheBoard
{
    public class UnitsOnRailsStore
    {
        private Dictionary<LineNode, Dictionary<string, (float Projection, float Size)>> unitsOnLines = new();

        public void SetUnitAtLine(LineNode line, string id, float projection, float size)
        {
            if (unitsOnLines.TryGetValue(line, out var unis) && unis != null)
            {
                unis.AddOrReplace(id, (projection, size));
                return;
            }

            var newUnits = new Dictionary<string, (float Projection, float Size)>
            {
                {id, (projection, size)}
            };
            
            unitsOnLines.Add(line, newUnits);
        }

        public LineUnit[] GetUnits(LineNode line)
        {
            if (unitsOnLines.TryGetValue(line, out var unis) && unis?.Count > 0)
                return unis.Select(pair => new LineUnit
                {
                    Id = pair.Key,
                    Projection = pair.Value.Projection,
                    Size = pair.Value.Size
                }).ToArray();

            return null;
        }
        
        public void RemoveUnitFormLine(LineNode line, string id)
        {
            if (unitsOnLines.TryGetValue(line, out var unis) && unis?.Count > 0)
                unis.Remove(id);
        }
        
        public void Clear()
        {
            unitsOnLines.Clear();
        }
    }

    public struct LineUnit
    {
        public string Id;
        public float Projection;
        public float Size;
    }
}