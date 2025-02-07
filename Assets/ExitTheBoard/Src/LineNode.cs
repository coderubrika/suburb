using System.Linq;
using UnityEngine;

namespace ExitTheBoard
{
    public class LineNode
    {
        private PointNode startPoint;
        private PointNode endPoint;

        public LineNode[] FromStartLines { get; private set; }
        public LineNode[] FromEndLines { get; private set; }

        public LineNode(PointNode startPoint, PointNode endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public void SetNeighboursStartLines(LineNode[] lines)
        {
            FromStartLines = lines;
        }
        
        public void SetNeighboursEndLines(LineNode[] lines)
        {
            FromEndLines = lines;
        }

        public LineTrack GetTrack()
        {
            return new LineTrack(startPoint.Position, endPoint.Position);
        }

        public LineTrack GetCollinearLineTrack()
        {
            return new LineTrack(GetFarPointFromPointNode(startPoint), GetFarPointFromPointNode(endPoint));
        }
        
        private Vector3 GetFarPointFromPointNode(PointNode pointNode)
        {
            if (pointNode == startPoint)
            {
                foreach (var line in FromStartLines)
                {
                    if (!CheckCollinear(this, line)) 
                        continue;
                    
                    return line.GetFarPointFromPointNode(line.startPoint == startPoint 
                        ? line.endPoint : line.startPoint);
                }

                return startPoint.Position;
            }
            
            foreach (var line in FromEndLines)
            {
                if (!CheckCollinear(this, line)) 
                    continue;
                return line.GetFarPointFromPointNode(line.startPoint == endPoint 
                    ? line.endPoint : line.startPoint);
            }

            return endPoint.Position;
        }

        private bool CheckCollinear(LineNode a, LineNode b)
        {
            Vector3 dirA = a.GetTrack().Direction;
            Vector3 dirB = b.GetTrack().Direction;
            Vector2 pair = GetNotNullPair(dirA, dirB);
            if (pair == Vector2.zero)
                return false;

            float lambda = pair[1] / pair[0];
            return dirA * lambda == dirB;
        }

        private Vector2 GetNotNullPair(Vector3 a, Vector3 b)
        {
            for (int i = 0; i < 3; i++)
            {
                if (a[i] != 0 && b[i] != 0)
                    return new Vector2(a[i], b[i]);
            }
            
            return Vector2.zero;
        }
    }
}