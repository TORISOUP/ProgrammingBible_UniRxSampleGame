using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UniRxSampleGame.Enemies
{
    // 敵の移動制御コンポーネント
    public class EnemyMove : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;

        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpPower;
        [SerializeField] private float _damageReactionPower = 2;

        // 敵の向いている方向
        private readonly ReactiveProperty<bool> _isRightDirection = new ReactiveProperty<bool>(false);
        private float _defaultScale;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _defaultScale = transform.localScale.x;
        }

        private void Start()
        {
            _isRightDirection.AddTo(this);

            // 向きに合わせてアニメーション方向を変える
            _isRightDirection
                .Subscribe(x =>
                {
                    if (x)
                    {
                        var scale = transform.localScale;
                        scale.x = -_defaultScale;
                        transform.localScale = scale;
                    }
                    else
                    {
                        var scale = transform.localScale;
                        scale.x = _defaultScale;
                        transform.localScale = scale;
                    }
                }).AddTo(this);
        }
        
        private void FixedUpdate()
        {
            // 止まったら向きを反転する
            if (_rigidbody2D.velocity.sqrMagnitude < 0.001f)
            {
                _isRightDirection.Value = !_isRightDirection.Value;
            }

            // 移動する
            var current = _rigidbody2D.velocity;
            var moveVelocity = Vector3.right * _moveSpeed * (_isRightDirection.Value ? 1 : -1);

            // 普段の移動速度より大きい速度で移動している場合は何もしない
            // （吹っ飛び中は速度のを制御をしない）
            if (Mathf.Abs(current.x) > _moveSpeed)
            {
                moveVelocity.x = current.x;
            }

            // ランダムでジャンプする
            var jump = Random.Range(0, 100) < 10 ? Vector3.up * _jumpPower : Vector3.zero;
            var next = moveVelocity + new Vector3(0, current.y, 0) + jump;
            _rigidbody2D.velocity = next;
        }

        // 吹っ飛ばす
        public void BlowAway(Vector3 direction)
        {
            _rigidbody2D.velocity = direction * _damageReactionPower;
        }
    }
}