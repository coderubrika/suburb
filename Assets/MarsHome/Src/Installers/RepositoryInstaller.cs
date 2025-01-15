using Suburb.Common;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Suburb.Installers
{
    [CreateAssetMenu(fileName = "RepositoryInstaller", menuName = "Installers/RepositoryInstaller")]
    public class RepositoryInstaller : ScriptableObjectInstaller<RepositoryInstaller>
    {
        [SerializeField] private PrefabsRepository prefabsRepository;
        [SerializeField] private GameSettingsRepository gameSettingsRepository;
        [SerializeField] private LanguagesRepository languagesRepository;
        [SerializeField] private PrefabGroupsRepository prefabGroupsRepository;
        
        public override void InstallBindings()
        {
            Container.BindInstances(prefabsRepository);
            Container.BindInstances(gameSettingsRepository);
            Container.BindInstances(languagesRepository);
            Container.BindInstances(prefabGroupsRepository);
        }
    }
}
