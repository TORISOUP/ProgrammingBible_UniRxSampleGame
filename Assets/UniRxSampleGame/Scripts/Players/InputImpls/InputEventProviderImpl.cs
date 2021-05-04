using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace UniRxSampleGame.Players.InputImpls
{
    public sealed class InputEventProviderImpl : MonoBehaviour, IInputEventProvider
    {
        // 長押しだと判定するまでの時間
        private static readonly float LongPressSeconds = 0.25f;

        #region IInputEventProvider

        public IObservable<Unit> OnLightAttach => _lightAttackSubject;
        public IObservable<Unit> OnStrongAttack => _strongAttackSubject;
        public IReadOnlyReactiveProperty<bool> IsJump => _jump;
        public IReadOnlyReactiveProperty<Vector3> MoveDirection => _move;

        #endregion

        // イベント発行に利用するSubjectやReactiveProperty
        private readonly Subject<Unit> _lightAttackSubject = new Subject<Unit>();
        private readonly Subject<Unit> _strongAttackSubject = new Subject<Unit>();
        private readonly ReactiveProperty<bool> _jump = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<Vector3> _move = new ReactiveProperty<Vector3>();

        private void Start()
        {
            // OnDestroy時にDispose()されるように登録
            _lightAttackSubject.AddTo(this);
            _strongAttackSubject.AddTo(this);
            _jump.AddTo(this);
            _move.AddTo(this);
            
            // 攻撃ボタンの長押し具合で弱/強攻撃を分岐
            this.UpdateAsObservable()
                .Select(_ => Input.GetButton("Attack"))
                .DistinctUntilChanged()
                .TimeInterval()
                .Skip(1)
                .Subscribe(t =>
                {
                    // 攻撃ボタンを押した瞬間のイベントは無視
                    if (t.Value) return;

                    // 攻撃ボタンを押してから離すまでの時間で判定
                    if (t.Interval.TotalSeconds >= LongPressSeconds)
                    {
                        _strongAttackSubject.OnNext(Unit.Default);
                    }
                    else
                    {
                        _lightAttackSubject.OnNext(Unit.Default);
                    }
                }).AddTo(this);
        }

        private void Update()
        {
            // ジャンプボタンの押し具合を反映
            _jump.Value = Input.GetButton("Jump");
            
            // 移動入力をベクトルに変換して反映
            // ReactiveProperty.SetValueAndForceNotifyを使うと強制的にメッセージ発行できる
            _move.SetValueAndForceNotify(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        }
        
    }
}