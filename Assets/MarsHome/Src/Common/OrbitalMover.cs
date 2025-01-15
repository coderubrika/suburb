using UnityEngine;

namespace Suburb.Common
{
    public class OrbitalMover
    {
        private readonly Quaternion rotationDelta;
        private readonly float radius;
        private readonly float scale;

        private PrefabRef reference;
        private Transform center;
        private Transform origin;
        private Transform appendage;
        private Quaternion lastRotation;
        
        public OrbitalMover(Vector3 rotationAxis, float rotationSpeed, float radius, float scale, Quaternion startRotation)
        {
            this.radius = radius;
            this.scale = scale;
            rotationDelta = Quaternion.Euler(rotationAxis * rotationSpeed * Time.deltaTime);
            lastRotation = startRotation;
        }
        
        public void Connect(Transform center, PrefabRef reference)
        {
            this.reference = reference;
            this.center = center;
            origin = reference.transform;
            origin.position = center.position;
            appendage = reference.Refs["Appendage"] as Transform;
            appendage.localPosition = Vector3.right * radius;
            appendage.localScale = Vector3.one * scale;
            origin.rotation = lastRotation;
        }

        public void Update()
        {
            origin.localRotation *= rotationDelta;
        }

        public PrefabRef Disconnect()
        {
            lastRotation = origin.rotation;
            return reference;
        }
    }
}