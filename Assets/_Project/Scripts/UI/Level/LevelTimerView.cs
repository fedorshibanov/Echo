using _Project.Scripts.Animations;
using _Project.Scripts.RecordSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VContainer;

namespace _Project.Scripts.UI.Level
{
    public class LevelTimerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;

        private IRecordService _recordService;
        private bool _started;
        private Sequence _timerTween;

        [Inject]
        private void Init(IRecordService recordService) => _recordService = recordService;

        private void Start()
        {
            _started = true;
            Subscribe();
        }

        private void OnEnable()
        {
            if (_started) Subscribe();
        }

        private void OnDisable()
        {
            _timerTween?.Kill();
            _timerTween = null;

            if (!_started) return;

            _recordService.RemainingSecondsChanged -= ShowRemainingSeconds;
        }

        private void Subscribe()
        {
            _recordService.RemainingSecondsChanged += ShowRemainingSeconds;
            _timerText.SetText("{0}", _recordService.RemainingSeconds);
        }

        private void ShowRemainingSeconds(int seconds)
        {
            _timerTween?.Kill();
            _timerText.SetText("{0}", seconds);
            _timerTween = Anim.Level.TimerDigitBounce(_timerText.transform);
        }
    }
}
