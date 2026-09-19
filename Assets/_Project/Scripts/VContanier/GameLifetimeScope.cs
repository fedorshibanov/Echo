using VContainer;
using VContainer.Unity;

namespace _Project.Scripts.VContanier
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerInputSystem>(Lifetime.Singleton);
        }
    }
}
