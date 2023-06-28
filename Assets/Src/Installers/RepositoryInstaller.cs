using Suburb.Common;
using UnityEngine;
using Zenject;

namespace Suburb.Installers
{
    [CreateAssetMenu(fileName = "RepositoryInstaller", menuName = "Installers/RepositoryInstaller")]
    public class RepositoryInstaller : ScriptableObjectInstaller<RepositoryInstaller>
    {
        [SerializeField] private ResourcesRepository resourcesRepository;
        [SerializeField] private GameSettingsRepository gameSettingsRepository;

        public override void InstallBindings()
        {
            Container.BindInstances(resourcesRepository);
            Container.BindInstances(gameSettingsRepository);
        }
    }
}
