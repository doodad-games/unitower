using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    static Spawner _i;

    public static void MaybeStartSpawning()
    {
        if (_i._spawning) return;
        _i._spawning = true;

        _i.StartCoroutine(_i.SpawnTimer());
    }

    #pragma warning disable CS0649
    [SerializeField] Transform _spawnPoint;
    [SerializeField] Transform _despawnPoint;
    #pragma warning restore CS0649

    bool _spawning;

    Vector3 _despawnPointVec;

    void Awake() => _i = this;

    void Start() => _despawnPointVec = _despawnPoint.position;

    IEnumerator SpawnTimer()
    {
        var spawnAmount = GameController.Generation.EnemiesToSpawn == 0
            ? int.MaxValue 
            : GameController.Generation.EnemiesToSpawn;
        
        var timePerSpawn = C.EnemySpawnRateInitial;

        var life = C.EnemyLifeInitial;

        var speed = C.EnemySpeedInitial;

        for (var i = 1; i != spawnAmount + 1; ++i)
        {
            yield return new WaitForSeconds(Mathf.Max(
                C.EnemySpawnRateMin,
                timePerSpawn
            ));

            Instantiate(
                C.Enemy,
                _spawnPoint.position,
                Quaternion.identity
            )
                .GetComponent<Enemy>()
                .Init(_despawnPointVec, life, speed);
            
            if (i % C.EnemySpawnRateMultipleEvery == 0)
                timePerSpawn *= C.EnemySpawnRateMultiple;
            if (i % C.EnemyLifeIncrementEvery == 0)
                life += C.EnemyLifeIncrementAmount;
            if (i % C.EnemySpeedIncrementEvery == 0)
                speed += C.EnemySpeedIncrementAmount;
        }

        GameController.finishedSpawning = true;
    }
}
