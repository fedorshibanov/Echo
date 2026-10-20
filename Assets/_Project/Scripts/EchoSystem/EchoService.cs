using System.Collections.Generic;

namespace _Project.Scripts.EchoSystem
{
    public class EchoService
    {
        private List<Echo> _echoService = new();
        
        public void AddEcho(Echo echo)
        {
            _echoService.Add(echo);
        }

        public List<Echo> GetEchos()
        {
            return _echoService;
        }
    }
}