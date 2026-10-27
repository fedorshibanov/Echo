using _Project.Scripts.RecordSystem;
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
            if (!_started) return;

            _recordService.RemainingSecondsChanged -= ShowRemainingSeconds;
        }

        private void Subscribe()
        {
            _recordService.RemainingSecondsChanged += ShowRemainingSeconds;
            ShowRemainingSeconds(_recordService.RemainingSeconds);
        }

        private void ShowRemainingSeconds(int seconds) => _timerText.SetText("{0}", seconds);
    }
}
