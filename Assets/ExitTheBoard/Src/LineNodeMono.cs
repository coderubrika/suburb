using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExitTheBoard
{
    public class LineNodeMono : MonoBehaviour
    {
        [SerializeField] private List<LineNodeMono> fromStartLines = new();
        [SerializeField] private List<LineNodeMono> fromEndLines = new();
        [SerializeField] private PointNodeMono startPoint;
        [SerializeField] private PointNodeMono endPoint;

        private LineNode lineNode;
        
        private static int idx;

        public LineNode Scan()
        {
            if (lineNode != null)
                return lineNode;
            
            PointNode startPointNode = startPoint.Scan();
            PointNode endPointNode = endPoint.Scan();
            lineNode = new LineNode(startPointNode, endPointNode);

            LineNode[] startLineNodes = fromStartLines
                .Select(lineMono => lineMono.Scan())
                .ToArray();
            
            LineNode[] endLineNodes = fromEndLines
                .Select(lineMono => lineMono.Scan())
                .ToArray();
            
            lineNode.SetNeighboursStartLines(startLineNodes);
            lineNode.SetNeighboursEndLines(endLineNodes);
            
            return lineNode;
        }
        
        public void SetStartPoint(PointNodeMono point)
        {
            startPoint = point;
            if (endPoint != null)
                return;

            GameObject pointGO = new GameObject("PointEnd", typeof(PointNodeMono));
            PointNodeMono pointNode = pointGO.GetComponent<PointNodeMono>();

            endPoint = pointNode;
            
            endPoint.AddNeighbourPoint(startPoint);
            startPoint.AddNeighbourPoint(endPoint);

            pointNode.transform.parent = transform;
            pointNode.transform.localScale = Vector3.one;
            pointNode.transform.localRotation = Quaternion.identity;
            pointNode.transform.position = startPoint.transform.position;
        }
        
        public void SetEndPoint(PointNodeMono point)
        {
            endPoint = point;
            
            if (startPoint != null)
                return;
            
            GameObject pointGO = new GameObject("StartEnd", typeof(PointNodeMono));
            PointNodeMono pointNode = pointGO.GetComponent<PointNodeMono>();

            startPoint = pointNode;
            
            endPoint.AddNeighbourPoint(startPoint);
            startPoint.AddNeighbourPoint(endPoint);

            pointNode.transform.parent = transform;
            pointNode.transform.localScale = Vector3.one;
            pointNode.transform.localRotation = Quaternion.identity;
            pointNode.transform.position = startPoint.transform.position;
        }
        
        [ContextMenu("CreateLineFromStart")]
        private void CreateLineFromStart()
        {
            GameObject lineGO = new GameObject($"Line ({GetNextIdx()})", typeof(LineNodeMono));
            LineNodeMono line = lineGO.GetComponent<LineNodeMono>();
            
            line.SetStartPoint(startPoint);
            line.fromStartLines.Add(this);
            
            foreach (var startLine in fromStartLines)
            {
                line.fromStartLines.Add(startLine);
                
                if (startLine.startPoint == startPoint)
                    startLine.fromStartLines.Add(line);
                else if (startLine.endPoint == startPoint)
                    startLine.fromEndLines.Add(line);
            }
            
            fromStartLines.Add(line);
        }
        
        [ContextMenu("CreateLineFromEnd")]
        private void CreateLineFromEnd()
        {
            GameObject lineGO = new GameObject($"Line ({GetNextIdx()})", typeof(LineNodeMono));
            LineNodeMono line = lineGO.GetComponent<LineNodeMono>();
            
            line.SetStartPoint(endPoint);
            line.fromStartLines.Add(this);
            
            foreach (var endLine in fromEndLines)
            {
                line.fromStartLines.Add(endLine);
                
                if (endLine.startPoint == endPoint)
                    endLine.fromStartLines.Add(line);
                else if (endLine.endPoint == endPoint)
                    endLine.fromEndLines.Add(line);
            }
            
            fromEndLines.Add(line);
        }

        private void OnDrawGizmos()
        {
            if (startPoint == null || endPoint == null)
                return;

            Gizmos.color = new Color(221/255f, 103/255f, 32/255f);
            Gizmos.DrawLine(startPoint.transform.position, endPoint.transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            if (startPoint == null || endPoint == null)
                return;
            
            Gizmos.color = new Color(222/255f, 188/255f, 31/255f);
            Gizmos.DrawLine(startPoint.transform.position, endPoint.transform.position);
        }

        private static int GetNextIdx()
        {
            return ++idx;
        }
    }
}