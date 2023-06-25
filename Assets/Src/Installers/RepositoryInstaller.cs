using Suburb.Resources;
using UnityEngine;
using Zenject;

namespace Suburb.Installers
{
    [CreateAssetMenu(fileName = "RepositoryInstaller", menuName = "Installers/RepositoryInstaller")]
    public class RepositoryInstaller : ScriptableObjectInstaller<RepositoryInstaller>
    {
        [SerializeField] private ResourcesRepository resourcesRepository;

        public override void InstallBindings()
        {
            Container.BindInstances(resourcesRepository);
        }
    }
}
