using _Project.Scripts.UI.MainMenu;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Scripts.VContainer
{
    public class MainMenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private MainMenuController _mainMenuController;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_mainMenuController);
        }
    }
}