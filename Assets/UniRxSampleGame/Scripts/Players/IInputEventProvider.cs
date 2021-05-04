using System;
using UniRx;
using UnityEngine;

namespace UniRxSampleGame.Players
{
    // 入力操作イベントをObservable,IReadonlyReactivePropertyとして提供する
    public interface IInputEventProvider
    {
        // 移動操作
        IReadOnlyReactiveProperty<Vector3> MoveDirection { get; }

        // 弱攻撃
        IObservable<Unit> OnLightAttach { get; }

        // 強攻撃
        IObservable<Unit> OnStrongAttack { get; }

        // ジャンプ
        IReadOnlyReactiveProperty<bool> IsJump { get; }
    }
}