using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks; // UniTask v2.2.5
using UniRxSampleGame.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UniRxSampleGame.Managers
{
    // UniTask版の実装例（EnemyCoreもUniTask版にする必要あり）
    public sealed class EnemyManagerWithUniTask : MonoBehaviour
    {
        [SerializeField] private EnemyCoreWithUniTask _prefab;
        [SerializeField] private Transform[] _enemySpawnPoints;
        [SerializeField] private ScoreManager _scoreManager;
        private readonly List<EnemyCoreWithUniTask> _enemies = new List<EnemyCoreWithUniTask>();
        private CancellationTokenSource _cancellationTokenSource;

        // 敵をすべて消して再生成する
        public void ResetEnemies()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            foreach (var enemyCore in _enemies)
            {
                if (enemyCore != null) Destroy(enemyCore.gameObject);
            }

            _enemies.Clear();

            // 敵を生成する
            EnemySpawnLoopAsync(_cancellationTokenSource.Token).Forget();
        }

        // 定期的に敵を生成する
        private async UniTaskVoid EnemySpawnLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var spawnPoint = _enemySpawnPoints[Random.Range(0, _enemySpawnPoints.Length)];
                var enemy = Instantiate(_prefab, spawnPoint.position, spawnPoint.rotation);

                // 敵の死亡通知購読
                WaitForKilledAsync(enemy, token).Forget();

                _enemies.Add(enemy);

                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(2, 5)), cancellationToken: token);
            }
        }

        // 敵が死亡したらスコアを加算する
        private async UniTaskVoid WaitForKilledAsync(EnemyCoreWithUniTask enemy, CancellationToken token)
        {
            var score = await enemy.OnKilledAsync.AttachExternalCancellation(token);
            _scoreManager.AddScore(score);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}