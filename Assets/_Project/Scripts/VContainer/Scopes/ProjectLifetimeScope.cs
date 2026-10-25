using _Project.Scripts.SaveSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Scripts.VContainer.Scopes
{
    public class ProjectLifetimeScope : LifetimeScope
    {
        private static ProjectLifetimeScope _instance;
        
        protected override void Awake()
        {
            if (_instance != null && _instance != this)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }

            _instance = this;

            DontDestroyOnLoad(gameObject);

            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ISceneChanger, SceneChanger>(Lifetime.Singleton);
            builder.Register<ISaveService, JsonSaveService>(Lifetime.Scoped);
            
            builder.RegisterEntryPoint<ProjectEntryPoint>();
        }
    }
}
