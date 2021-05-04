using Cysharp.Threading.Tasks; // UniTask v2.2.5
using UniRx;
using UniRxSampleGame.Damages;
using UnityEngine;

namespace UniRxSampleGame.Enemies
{
    // UniTask版の実装例（EnemyManagerもUniTask版にする必要あり）
    public sealed class EnemyCoreWithUniTask : MonoBehaviour, IDamageApplicable
    {
        // デフォルトHP
        [SerializeField] private int _maxHp = 2;

        // 倒したときに得られるスコア
        [SerializeField] private int _score = 10;

        // 現在のHP
        private ReactiveProperty<int> _hp;
        private EnemyMove _enemyMove;


        // 死亡通知用のUniTaskCompletionSource
        private readonly UniTaskCompletionSource<int> _onKilledUniTaskCompletionSource =
            new UniTaskCompletionSource<int>();

        public UniTask<int> OnKilledAsync => _onKilledUniTaskCompletionSource.Task;

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
            _onKilledUniTaskCompletionSource.TrySetResult(_score);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _onKilledUniTaskCompletionSource.TrySetCanceled();
        }
    }
}