using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRxSampleGame.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UniRxSampleGame.Managers
{
    // 敵の管理を行う
    public sealed class EnemyManager : MonoBehaviour
    {
        [SerializeField] private EnemyCore _prefab;
        [SerializeField] private Transform[] _enemySpawnPoints;
        [SerializeField] private ScoreManager _scoreManager;
        private readonly List<EnemyCore> _enemies = new List<EnemyCore>();
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
    
        // 敵をすべて消して再生成する
        public void ResetEnemies()
        {
            _compositeDisposable.Clear();
    
            foreach (var enemyCore in _enemies)
            {
                if (enemyCore != null) Destroy(enemyCore.gameObject);
            }
    
            _enemies.Clear();
    
            StopAllCoroutines();
            StartCoroutine(EnemySpawnCoroutine());
        }
    
        // 定期的に敵を生成するコルーチン
        private IEnumerator EnemySpawnCoroutine()
        {
            while (true)
            {
                var spawnPoint = _enemySpawnPoints[Random.Range(0, _enemySpawnPoints.Length)];
                var enemy = Instantiate(_prefab, spawnPoint.position, spawnPoint.rotation);
    
                // 敵の死亡状態の購読
                enemy.OnKilledAsync
                    .Subscribe(x => _scoreManager.AddScore(x))
                    .AddTo(_compositeDisposable);
    
                _enemies.Add(enemy);
    
                yield return new WaitForSeconds(Random.Range(2, 5));
            }
        }
    }
}