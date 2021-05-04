using System;
using TMPro;
using UniRx;
using UniRxSampleGame.Managers;
using UnityEngine;

namespace UniRxSampleGame.Presenters
{
    // ゲーム終了時のUI表示
    public sealed class ResultPresenter : MonoBehaviour
    {
        // 画面右上の常駐のスコア表示Canvas
        [SerializeField] private Canvas _scoreCanvas;
        // 画面全体を覆う最終結果を表示するためのCanvas
        [SerializeField] private Canvas _resultCanvas;
        // スコア表示のText
        [SerializeField] private TMP_Text _text;

        [SerializeField] private GameStateManager _gameStateManager;
        [SerializeField] private ScoreManager _scoreManager;

        private void Start()
        {
            // ゲームステートの変動に合わせて
            // 各Canvasの表示/非表示を切り替える
            _gameStateManager
                .State
                .Subscribe(x =>
                {
                    switch (x)
                    {
                        case GameState.Playing:
                            Hide();
                            break;
                        case GameState.Result:
                            Show(_scoreManager.Score.Value);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(x), x, null);
                    }
                    
                }).AddTo(this);
        }

        // リザルト画面を隠す
        private void Hide()
        {
            _scoreCanvas.enabled = true;
            _resultCanvas.enabled = false;
        }

        // リザルト画面を出す
        private void Show(int score)
        {
            _scoreCanvas.enabled = false;
            _resultCanvas.enabled = true;
            _text.text = $"Your score is {score}";
        }
    }
}