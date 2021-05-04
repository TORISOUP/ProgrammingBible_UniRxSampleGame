using UniRx;
using UnityEngine;

namespace UniRxSampleGame.Managers
{
    // 点数の管理
    public sealed class ScoreManager : MonoBehaviour
    {
        private readonly ReactiveProperty<int> _score = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<int> Score => _score;

        private void Start()
        {
            _score.AddTo(this);
        }

        // スコアを加算する
        public void AddScore(int score)
        {
            _score.Value += score;
        }

        // スコアリセット
        public void ResetScore()
        {
            _score.Value = 0;
        }
    }
}