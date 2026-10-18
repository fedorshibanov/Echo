using System;
using _Project.Scripts.SaveSystem;
using VContainer.Unity;

namespace _Project.Scripts.VContainer
{
    public class ProjectEntryPoint : IStartable
    {
        private readonly ISceneChanger _sceneChanger;
        private readonly ISaveService _saveService;

        public ProjectEntryPoint(ISceneChanger sceneChanger, ISaveService saveService)
        {
            _sceneChanger = sceneChanger;
            _saveService = saveService;
        }
        
        public void Start()
        {
            int id = _saveService.Load().levelId;
            _sceneChanger.LoadLevelId(_saveService.Load().levelId);
        }
    }
}