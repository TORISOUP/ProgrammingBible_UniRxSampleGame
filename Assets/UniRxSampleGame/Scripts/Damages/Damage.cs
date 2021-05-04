using System;
using UnityEngine;

namespace UniRxSampleGame.Damages
{
    // ダメージを表現する構造体
    public readonly struct Damage : IEquatable<Damage>
    {
        // ダメージ値
        public int Value { get; }
        // 吹っ飛ぶ方向
        public Vector3 BlowsAwayDirection { get; }

        public Damage(int value, Vector3 blowsAwayDirection)
        {
            Value = value;
            BlowsAwayDirection = blowsAwayDirection;
        }

        public bool Equals(Damage other)
        {
            return Value == other.Value && BlowsAwayDirection.Equals(other.BlowsAwayDirection);
        }

        public override bool Equals(object obj)
        {
            return obj is Damage other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Value * 397) ^ BlowsAwayDirection.GetHashCode();
            }
        }
    }
}