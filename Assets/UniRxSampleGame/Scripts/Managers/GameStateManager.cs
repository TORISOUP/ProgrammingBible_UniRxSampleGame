using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace UniRxSampleGame.Managers
{
    // ゲームの進行管理を行う
    public sealed class GameStateManager : MonoBehaviour
    {
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private ScoreManager _scoreManager;

        // ゲームが進行中であるか
        private readonly ReactiveProperty<GameState> _state 
            = new ReactiveProperty<GameState>(GameState.Playing);
        public IReadOnlyReactiveProperty<GameState> State => _state;

        private void Start()
        {
            _state.AddTo(this);

            ResetGame();

            // プレイヤが死亡したらゲーム終了
            _playerManager
                .OnPlayerDeadAsObservable
                .Subscribe(_ => _state.Value = GameState.Result)
                .AddTo(this);

            // Rキーでいつでもリセット
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.R))
                // 連打防止
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => ResetGame())
                .AddTo(this);
        }

        private void ResetGame()
        {
            _playerManager.RespawnPlayer();
            _enemyManager.ResetEnemies();
            _scoreManager.ResetScore();
            _state.Value = GameState.Playing;
        }
    }
}