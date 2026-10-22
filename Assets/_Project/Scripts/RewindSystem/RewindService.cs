using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Animations;
using _Project.Scripts.EchoSystem;
using _Project.Scripts.PlayerSystem;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.RewindSystem
{
    public class RewindService
    {
        private readonly EchoService _echoService;
        private readonly Player _player;
        private readonly IReadOnlyList<IRewindable> _items;

        public RewindService(EchoService echoService, Player player, IReadOnlyList<IRewindable> items)
        {
            _echoService = echoService;
            _player = player;
            _items = items;
        }

        public Task RewindAll(Vector3 spawnPoint) => Play(spawnPoint, true, true);
        public Task RewindEntities(Vector3 spawnPoint) => Play(spawnPoint, true, false);
        public Task RewindItems() => Play(Vector3.zero, false, true);

        private async Task Play(Vector3 spawnPoint, bool entities, bool items)
        {
            SetControl(false);
            var seq = DOTween.Sequence();

            if (entities)
            {
                var targets = _echoService.GetEchos().Select(e => e.transform).Prepend(_player.transform).ToList();
                seq.Join(Anim.Level.CollapseMoveExpand(targets, spawnPoint, false));
            }
            if (items)
            {
                var targets = _items.Select(i => ((Component)i).transform).ToList();
                seq.Join(Anim.Level.CollapseMoveExpand(targets, spawnPoint, onHidden: i => _items[i].Rewind()));
            }

            await seq.AsyncWaitForCompletion();

            if (entities)
                foreach (var echo in _echoService.GetEchos())
                    echo.Controller.MoveEcho();

            SetControl(true);
        }

        private void SetControl(bool on)
        {
            Toggle(_player.Rigidbody, _player.Controller, on);
            foreach (var echo in _echoService.GetEchos())
                Toggle(echo.Rigidbody, echo.Controller, on);
        }

        private static void Toggle(Rigidbody rb, Behaviour controller, bool on)
        {
            rb.isKinematic = !on;
            controller.enabled = on;
        }
    }
}