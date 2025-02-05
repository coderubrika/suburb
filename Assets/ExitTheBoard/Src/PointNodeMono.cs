using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ExitTheBoard
{
    public class PointNodeMono : MonoBehaviour
    {
        [SerializeField] private List<PointNodeMono> neighboursPoints = new();

        [ContextMenu("Duplicate")]
        private void Duplicate()
        {
            string pattern = @"\((-?\d+)\)";
            Match match = Regex.Match(name, pattern);
            int idx = match.Success ? Int32.Parse(match.Groups[1].Value)+1 : 0; 
            GameObject pointGO = new GameObject($"Point ({idx})", typeof(PointNodeMono));
            pointGO.transform.parent = transform.parent;
            pointGO.transform.localScale = transform.localScale;
            pointGO.transform.localRotation = transform.localRotation;
            pointGO.transform.position = transform.position;
            
            PointNodeMono neighbourPoint = pointGO.GetComponent<PointNodeMono>();
            AddNeighbourPoint(neighbourPoint);
            neighbourPoint.AddNeighbourPoint(this);
            
        }

        public void AddNeighbourPoint(PointNodeMono neighbourPoint)
        {
            neighboursPoints.Add(neighbourPoint);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (var point in neighboursPoints)
                Gizmos.DrawLine(transform.position, point.transform.position);
        }
    }
}