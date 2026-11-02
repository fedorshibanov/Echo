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

        public Task RewindAll(Vector3 spawnPoint, Quaternion spawnRotation) => Play(spawnPoint, spawnRotation, true, true);
        public Task RewindEntities(Vector3 spawnPoint, Quaternion spawnRotation) => Play(spawnPoint, spawnRotation, true, false);
        public Task RewindItems() => Play(Vector3.zero, Quaternion.identity, false, true);

        private async Task Play(Vector3 spawnPoint, Quaternion spawnRotation, bool entities, bool items)
        {
            SetControl(false);
            var itemBodies = items
                ? _items.SelectMany(i => ((Component)i).GetComponentsInChildren<Rigidbody>(true))
                    .Distinct().ToDictionary(rb => rb, rb => rb.isKinematic)
                : new Dictionary<Rigidbody, bool>();
            foreach (var body in itemBodies.Keys)
            {
                if (!body.isKinematic)
                {
                    body.linearVelocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                }
                body.isKinematic = true;
            }

            try
            {
                var seq = DOTween.Sequence();
                if (entities)
                {
                    var targets = _echoService.GetEchos().Select(e => e.transform).Prepend(_player.transform).ToList();
                    seq.Join(Anim.Level.CollapseMoveExpand(targets, spawnPoint, false, onHidden: i =>
                        targets[i].SetPositionAndRotation(spawnPoint, spawnRotation)));
                }
                if (items)
                {
                    var targets = _items.Select(i => ((Component)i).transform).ToList();
                    seq.Join(Anim.Level.CollapseMoveExpand(targets, spawnPoint, onHidden: i => _items[i].Rewind()));
                }

                await seq.AsyncWaitForCompletion();

                Physics.SyncTransforms();
            }
            finally
            {
                foreach (var entry in itemBodies)
                    if (entry.Key != null)
                        entry.Key.isKinematic = entry.Value;

                SetControl(true);
            }

            if (entities)
                foreach (var echo in _echoService.GetEchos())
                    echo.Controller.MoveEcho();
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
            if (on)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            controller.enabled = on;
        }
    }
}
