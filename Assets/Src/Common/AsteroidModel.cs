using UnityEngine;

namespace Suburb.Common
{
    public class AsteroidModel
    {
        private readonly Vector3 scale;
        private readonly Vector3 ellipseHalf;
        private readonly float e;
        private readonly Vector3 rotationDelta;
        
        private Quaternion rotation;
        private Vector3 position;
        private Transform origin;
        private float positionTimer;
        
        public AsteroidModel(Vector3 ellipseHalf, float e, Vector3 rotationDelta, float scale)
        {
            this.ellipseHalf = ellipseHalf;
            this.e = e;
            this.rotationDelta = rotationDelta;
            this.scale = Vector3.one * scale;
        }

        public void SetOrigin(Transform origin)
        {
            this.origin = origin;
            origin.position = position;
            origin.localRotation = rotation;
            origin.localScale = scale;
        }
        
        public Transform GetOrigin()
        {
            return origin;
        }
        
        public void Update()
        {
            position = ellipseHalf * (1 - e * Mathf.Cos(positionTimer));
            rotation *= Quaternion.Euler(rotationDelta * Time.deltaTime * 2);
            origin.position = position;
            origin.localRotation = rotation;
            positionTimer += Time.deltaTime * 3;
        }
     }
}