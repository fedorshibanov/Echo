using VContainer;
using VContainer.Unity;

namespace _Project.Scripts.VContainer
{
    public class ProjectLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ISceneChanger, SceneChanger>(Lifetime.Singleton);
        }
    }
}
