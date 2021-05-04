using UniRx;
using UnityEngine;

namespace UniRxSampleGame.Players
{
    public sealed class PlayerAnimation : MonoBehaviour
    {
        private static readonly int HashSpeed = Animator.StringToHash("Speed");
        private static readonly int HashFallSpeed = Animator.StringToHash("FallSpeed");
        private static readonly int HashIsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int HashLightAttack = Animator.StringToHash("LightAttack");
        private static readonly int HashStrongAttack = Animator.StringToHash("StrongAttack");
        private static readonly int HashDeadState = Animator.StringToHash("Dead");

        private Animator _animator;
        private Rigidbody2D _rigidbody2D;
        private PlayerMove _playerMove;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _playerMove = GetComponent<PlayerMove>();

            // プレイヤが死んだら死亡ステートに遷移
            var core = GetComponent<PlayerCore>();
            core.IsDead
                .FirstOrDefault(x => x)
                .Subscribe(onNext: _ => _animator.Play(HashDeadState))
                .AddTo(this);
        }

        private void Update()
        {
            // 各種アニメーションの遷移
            _animator.SetBool(HashIsGrounded, _playerMove.IsGrounded.Value);

            var moveX = _rigidbody2D.velocity.x;
            var moveY = _rigidbody2D.velocity.y;

            _animator.SetFloat(HashFallSpeed, moveY);

            if (Mathf.Abs(moveX) > 0.05f)
            {
                var scale = transform.localScale;
                scale.x = moveX > 0 ? 1 : -1;
                transform.localScale = scale;
            }

            _animator.SetFloat(HashSpeed, Mathf.Abs(moveX));
            
            // 弱/強攻撃は1F以内に遷移できなかった場合はリセットする
            _animator.ResetTrigger(HashLightAttack);
            _animator.ResetTrigger(HashStrongAttack);
        }

        // 弱攻撃を試みる
        public void AttackLight()
        {
            _animator.SetTrigger(HashLightAttack);
        }

        // 強攻撃を試みる
        public void AttackStrong()
        {
            _animator.SetTrigger(HashStrongAttack);
        }
    }
}