using UniRx;
using UnityEngine;

namespace UniRxSampleGame.Players
{
    // プレイヤーの移動制御を行うコンポーネント
    public sealed class PlayerMove : MonoBehaviour
    {
        // 接地状態
        public IReadOnlyReactiveProperty<bool> IsGrounded => _isGrounded;

        // 落下中であるか
        public bool IsFall => _rigidbody2D.velocity.y < 0;


        // 移動速度
        [SerializeField] private float _dashSpeed = 3;

        // ジャンプ速度
        [SerializeField] private float _jumpSpeed = 5.5f;

        // Raycast時のプレイヤの高さを補正する
        [SerializeField] private float _characterHeightOffset = 0.15f;

        // 接地判定に利用するレイヤ設定
        [SerializeField] LayerMask _groundMask;


        private readonly RaycastHit2D[] _raycastHitResults = new RaycastHit2D[1];
        private readonly ReactiveProperty<bool> _isGrounded = new BoolReactiveProperty();
        private PlayerCore _playerCore;
        private Rigidbody2D _rigidbody2D;
        private bool _isJumpReserved;
        private IInputEventProvider _inputEventProvider;
        private bool _isMoveBlock;

        private void Start()
        {
            _isGrounded.AddTo(this);

            _rigidbody2D = GetComponent<Rigidbody2D>();
            _playerCore = GetComponent<PlayerCore>();
            _inputEventProvider = GetComponent<IInputEventProvider>();
        }

        // Rigidbodyに対する操作なのでFixedUpdateを用いる
        //
        // 操作イベントの更新はUpdateタイミングであるが、
        // 現在値を参照するだけであればFixedUpdate上で行っても問題はない
        private void FixedUpdate()
        {
            // 接地判定処理
            CheckGrounded();
            
            // 上書きする移動速度の値
            var vel = Vector3.zero;

            // プレイヤが死んでないなら操作を反映する
            if (!_playerCore.IsDead.Value)
            {
                // 操作イベントから得られた移動量
                var moveVector = GetMoveVector();

                // 移動操作を反映する
                if (moveVector != Vector3.zero && !_isMoveBlock)
                {
                    vel = moveVector * _dashSpeed;
                }

                // ジャンプ
                if (_inputEventProvider.IsJump.Value && _isGrounded.Value)
                {
                    vel += Vector3.up * _jumpSpeed;
                    _isJumpReserved = false;
                }
            }

            // 重力落下分を維持する
            vel += new Vector3(0, _rigidbody2D.velocity.y, 0);

            // 速度を更新
            _rigidbody2D.velocity = vel;

            // 落下中は無敵判定
            _playerCore.SetInvincible(IsFall);
        }

        // 操作イベントの値から移動量を決定する
        private Vector3 GetMoveVector()
        {
            var x = _inputEventProvider.MoveDirection.Value.x;
            if (x > 0.1f)
            {
                return Vector3.right;
            }
            else if (x < -0.1f)
            {
                return -Vector3.right;
            }
            else
            {
                return Vector3.zero;
            }
        }

        // 接地判定
        private void CheckGrounded()
        {
            // 地面に対してRaycastを飛ばして接地判定を行う
            var hitCount = Physics2D.CircleCastNonAlloc(
                origin: transform.position - new Vector3(0, _characterHeightOffset, 0),
                radius: 0.09f,
                direction: Vector2.down,
                results: _raycastHitResults,
                distance: 0.01f,
                layerMask: _groundMask);

            _isGrounded.Value = hitCount != 0;
        }

        // 移動不可フラグ
        public void BlockMove(bool isBlock)
        {
            _isMoveBlock = isBlock;
        }
    }
}