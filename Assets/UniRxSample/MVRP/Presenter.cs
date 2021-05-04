using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UniRxSample.MVRP
{
    // ModelとView(uGUI)を繋ぐオブジェクト
    public class Presenter : MonoBehaviour
    {
        // Modelオブジェクト
        [SerializeField] private Model _model;

        // 各UI要素
        [SerializeField] private Text _text;
        [SerializeField] private Button _upButton;
        [SerializeField] private Button _downButton;

        private void Start()
        {
            // Modelの値の変化をTextに反映
            _model.Score
                .Subscribe(x => _text.text = x.ToString())
                .AddTo(this);

            // Upが押されたら加算
            _upButton
                .OnClickAsObservable()
                .Subscribe(_ => _model.Score.Value++)
                .AddTo(this);

            // Downが押されたら減算
            _downButton
                .OnClickAsObservable()
                .Subscribe(_ => _model.Score.Value--)
                .AddTo(this);
        }
    }
}
