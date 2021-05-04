using System;
using UniRx;
using UniRxSampleGame.Damages;
using UnityEngine;

namespace UniRxSampleGame.Enemies
{
    // 敵本体を表すコンポーネント
    public sealed class EnemyCore : MonoBehaviour, IDamageApplicable
    {
        // デフォルトHP
        [SerializeField] private int _maxHp = 2;

        // 倒したときに得られるスコア
        [SerializeField] private int _score = 10;

        // 現在のHP
        private ReactiveProperty<int> _hp;
        private EnemyMove _enemyMove;

        // 死亡通知用のAsyncSubject
        private readonly AsyncSubject<int> _onKilledAsyncSubject = new AsyncSubject<int>();
        public IObservable<int> OnKilledAsync => _onKilledAsyncSubject;

        private void Awake()
        {
            _hp = new ReactiveProperty<int>(_maxHp);
            _hp.AddTo(this);
            _enemyMove = GetComponent<EnemyMove>();
        }

        private void Start()
        {
            // HPが0以下になったら死亡する
            _hp.Where(x => x <= 0)
                .Take(1)
                .Subscribe(_ => OnDead())
                .AddTo(this);
        }

        // ダメージを受けたときの処理
        public void ApplyDamage(in Damage damage)
        {
            // HPに反映
            _hp.Value -= damage.Value;

            // 吹っ飛ぶ
            _enemyMove.BlowAway(damage.BlowsAwayDirection);
        }


        private void OnDead()
        {
            // 死んだら通知を発行してDestroy()
            _onKilledAsyncSubject.OnNext(_score);
            _onKilledAsyncSubject.OnCompleted();
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            // Destroy時にDispose()することで
            // Subscribeを中断させることができる
            _onKilledAsyncSubject.Dispose();
        }
    }
}