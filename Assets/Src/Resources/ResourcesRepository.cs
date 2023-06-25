using Suburb.Common;
using Suburb.Interactables;
using UnityEngine;

namespace Suburb.Resources
{
    [CreateAssetMenu(fileName = "ResourcesRepository", menuName = "Repositories/ResourcesRepository")]
    public class ResourcesRepository : ScriptableObject
    {
        [SerializeField] Rover rover;
        [SerializeField] Land land;

        public Rover Rover => rover;
        public Land Land => land;
    }
}
