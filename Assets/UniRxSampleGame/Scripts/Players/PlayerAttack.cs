using UniRx;
using UniRx.Triggers;
using UniRxSampleGame.Damages;
using UnityEngine;

namespace UniRxSampleGame.Players
{
    // プレイヤの攻撃処理の管理コンポーネント
    public sealed class PlayerAttack : MonoBehaviour
    {
        // 現在の攻撃力
        private int _attackPower = 1;
        private IInputEventProvider _inputEventProvider;
        private PlayerAnimation _playerAnimation;
        private PlayerMove _playerMove;

        private readonly ReactiveProperty<bool> _isInAttack = new ReactiveProperty<bool>(false);

        private void Start()
        {
            _isInAttack.AddTo(this);

            _inputEventProvider = GetComponent<IInputEventProvider>();
            _playerAnimation = GetComponent<PlayerAnimation>();
            _playerMove = GetComponent<PlayerMove>();

            // 操作イベントの購読
            SubscribeInputEvent();

            // アニメーションイベントの購読
            SubscribeAnimationEvent();

            // 衝突イベントの購読
            SubscribeColliderEvent();

            // 攻撃中は移動不可フラグを立てる
            _isInAttack
                .Subscribe(x => _playerMove.BlockMove(x))
                .AddTo(this);

            OnAttackEndEvent();
        }

        #region 操作イベントの購読

        // 操作イベントの購読
        private void SubscribeInputEvent()
        {
            // 弱攻撃イベント
            _inputEventProvider.OnLightAttach
                // 接地中なら攻撃ができる
                .Where(_ => _playerMove.IsGrounded.Value)
                .Subscribe(_ => _playerAnimation.AttackLight())
                .AddTo(this);

            // 強攻撃イベント
            _inputEventProvider.OnStrongAttack
                // 接地中なら攻撃ができる
                .Where(_ => _playerMove.IsGrounded.Value)
                .Subscribe(_ => _playerAnimation.AttackStrong())
                .AddTo(this);
        }

        #endregion

        #region アニメーションイベントの購読

        // アニメーションイベントを購読する
        private void SubscribeAnimationEvent()
        {
            // ObservableStateMachineTrigger を用いることでAnimationControllerの
            // ステートの遷移を取得できる
            var animator = GetComponent<Animator>();
            var trigger = animator.GetBehaviour<ObservableStateMachineTrigger>();

            // 攻撃関係のステートマシンに入った
            trigger
                .OnStateMachineEnterAsObservable()
                .Subscribe(_ => _isInAttack.Value = true)
                .AddTo(this);

            // 攻撃関係のステートマシンから出た
            trigger
                .OnStateMachineExitAsObservable()
                .Subscribe(_ => _isInAttack.Value = false)
                .AddTo(this);
        }

        #endregion
        
        #region AnimationClipに設定されたイベントのコールバック

        // AnimationClip に付与されたAnimation Eventの購読
        // 攻撃モーションに合わせて当たり判定をON/OFFする
        public void OnLightAttackEvent()
        {
            _attackCollider1.enabled = true;
            _attackPower = 1; // 弱攻撃は攻撃力1
        }

        public void OnStrongAttackEvent()
        {
            _attackCollider1.enabled = true;
            _attackCollider2.enabled = true;
            _attackPower = 2; // 強攻撃は攻撃力2
        }

        public void OnAttackEndEvent()
        {
            _attackCollider1.enabled = false;
            _attackCollider2.enabled = false;
            _attackPower = 0;
        }

        #endregion

        #region 攻撃判定用コライダの衝突判定購読

        // ヒエラルキー上で子要素として存在する攻撃判定用コライダ
        [SerializeField] private Collider2D _attackCollider1;
        [SerializeField] private Collider2D _attackCollider2;

        // 各種衝突判定を購読する
        private void SubscribeColliderEvent()
        {
            // OnTriggerEnter2DAsObservableをコンポーネントに対して呼び出すと、
            // そのコンポーネントの付与されたGameObjectに自動的に
            // 衝突検知用のコンポーネントがAddComponentされる
            _attackCollider1.OnTriggerEnter2DAsObservable()
                .Merge(_attackCollider2.OnTriggerEnter2DAsObservable())
                .Subscribe(x =>
                {
                    // 武器に当たった相手がダメージを与えられる相手であるか
                    if (!x.TryGetComponent<IDamageApplicable>(out var d)) return;

                    // 斜め上方向にふっとばすベクトルを計算
                    var direction =
                        ((x.transform.position - transform.position).normalized + Vector3.up)
                        .normalized;
                    // 相手にダメージを与える
                    d.ApplyDamage(new Damage(_attackPower, direction));
                }).AddTo(this);

            // 落下中にプレイヤーに衝突した場合の判定
            this.OnCollisionEnter2DAsObservable()
                .Where(_ => _playerMove.IsFall)
                .Subscribe(x =>
                {
                    if (!x.gameObject.TryGetComponent<IDamageApplicable>(out var d)) return;
                    var direction =
                        ((x.transform.position - transform.position).normalized + Vector3.up)
                        .normalized;

                    // 相手を踏んだ場合はダメージ0でふっとばす
                    d.ApplyDamage(new Damage(0, direction));
                }).AddTo(this);
        }

        #endregion
    }
}