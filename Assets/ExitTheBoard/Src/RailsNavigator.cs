using System.Collections.Generic;
using UnityEngine;

namespace ExitTheBoard
{
    public class RailsNavigator
    {
        private readonly Dictionary<RailRider, LineNode> riders = new();
        
        public void MoveByDelta(RailRider rider, Vector3 delta)
        {
            
        }

        public void Init(RiderLine[] riderLines)
        {
            foreach (var riderLine in riderLines)
                riders.Add(riderLine.Rider, riderLine.Line.Scan());
        }

        public void Clear()
        {
            riders.Clear();
        }
    }
}