using UniRx;
using UniRx.Triggers;
using UniRxSampleGame.Enemies;
using UnityEngine;

namespace UniRxSampleGame.Players
{
    // プレイヤーの本体を表すコンポーネント
    public sealed class PlayerCore : MonoBehaviour
    {
        // 死んでいるか
        public IReadOnlyReactiveProperty<bool> IsDead => _isDead;
        private readonly ReactiveProperty<bool> _isDead = new ReactiveProperty<bool>();

        // 無的中か
        private bool _isInvincible;

        private void Start()
        {
            _isDead.AddTo(this);

            // 敵に衝突した場合は死ぬ
            this.OnCollisionEnter2DAsObservable()
                .Where(_ => !_isInvincible)
                .Where(x => x.gameObject.TryGetComponent<EnemyCore>(out _))
                .Subscribe(onNext: _ => _isDead.Value = true);
        }

        public void SetInvincible(bool isInvincible)
        {
            _isInvincible = isInvincible;
        }
    }
}