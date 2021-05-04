using TMPro;
using UniRx;
using UniRxSampleGame.Managers;
using UnityEngine;

namespace UniRxSampleGame.Presenters
{
    // 点数をUIに反映する
    public sealed class ScorePresenter : MonoBehaviour
    {
        [SerializeField] private ScoreManager _scoreManager;
        [SerializeField] private TMP_Text _text;

        private void Start()
        {
            _scoreManager
                .Score
                .Subscribe(x => _text.text = $"Score: {x}")
                .AddTo(this);
        }
    }
}