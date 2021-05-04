using UniRx;
using UnityEngine;

namespace UniRxSample.MVRP
{
    // 簡素なModel実装の例
    // データを保持するオブジェクト
    public class Model : MonoBehaviour
    {
        // ReactivePropertyで値を保持
        public readonly IntReactiveProperty Score 
            = new IntReactiveProperty(0);
    }
}