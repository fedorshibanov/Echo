using System.Collections.Generic;

namespace _Project.Scripts.EchoSystem
{
    public class EchoService
    {
        private List<EchoController> _echoService = new();
        
        public void AddEcho(EchoController echo)
        {
            _echoService.Add(echo);
        }

        public List<EchoController> GetEchos()
        {
            return _echoService;
        }
    }
}