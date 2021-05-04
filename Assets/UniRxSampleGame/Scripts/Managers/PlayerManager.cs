using System;
using UniRx;
using UniRxSampleGame.Players;
using UnityEngine;

namespace UniRxSampleGame.Managers
{
    // プレイヤーの管理
    public sealed class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PlayerCore _playerPrefab;
        [SerializeField] private Transform _spawnPoint;
        private PlayerCore _currentPlayer;
        
        // プレイヤの死亡通知
        private readonly Subject<Unit> _onPlayerDeadSubject = new Subject<Unit>();
        public IObservable<Unit> OnPlayerDeadAsObservable => _onPlayerDeadSubject;

        private void Start()
        {
            _onPlayerDeadSubject.AddTo(this);
        }

        public void RespawnPlayer()
        {
            // プレイヤの再生成
            if (_currentPlayer != null) Destroy(_currentPlayer.gameObject);
            _currentPlayer = Instantiate(_playerPrefab, _spawnPoint.position, _spawnPoint.rotation);

            // 今のプレイヤが死んだら通知する
            _currentPlayer
                .IsDead
                .Where(x => x)
                .Take(1)
                .Subscribe(_ => { _onPlayerDeadSubject.OnNext(Unit.Default); })
                .AddTo(this);
        }
    }
}