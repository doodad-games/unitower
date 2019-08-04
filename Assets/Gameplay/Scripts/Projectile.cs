using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    const int NumSingleParticles = 2;

    static Quaternion _inverse = Quaternion.Euler(0f, 180f, 0f);

    static TileType[] _enigmaticTypes = new TileType[]
    {   TileType.Burn
    ,   TileType.Slow
    ,   TileType.Speed
    ,   TileType.Spread
    };

    #pragma warning disable CS0649
    [SerializeField] ParticleSystem _singleParticles;
    [SerializeField] ParticleSystem _persistentParticles;
    #pragma warning restore CS0649

    bool _initd;
    bool _haveRotated;

    HashSet<int> _activeEnigmaAreas;
    HashSet<TileType> _activeEffects;
    Enemy _enemy;
    Transform _enemyTfm;
    Vector3 _dest;

    public void Init(Enemy enemy)
    {
        if (_initd) throw new System.NotImplementedException();
        _initd = true;

        _enemy = enemy;
        _enemyTfm = _enemy.transform;
        _dest = _enemyTfm.position;

        SoundController.Shoot();
    }

    public void AddEffect(TileType type, int area)
    {
        if (
            type == TileType.None ||
            (
                type == TileType.Enigma &&
                area == 0
            )
        ) throw new System.NotSupportedException();

        if (type != TileType.Enigma)
        {
            if (!_activeEffects.Contains(type))
            {
                var colour = C.TowerParticleColour[type];

                {
                    var main = _singleParticles.main;
                    main.startColor = colour;
                    _singleParticles.Emit(NumSingleParticles);
                }

                {
                    var main = _persistentParticles.main;
                    main.startColor = colour;
                    _persistentParticles.Emit(1);
                }

                _activeEffects.Add(type);
            }
        }
        else if (!_activeEnigmaAreas.Contains(area))
        {
            _activeEnigmaAreas.Add(area);

            AddEffect(
                _enigmaticTypes
                    .Where(_ => !_activeEffects.Contains(_))
                    .Skip(UnityEngine.Random.Range(
                        0,
                        _enigmaticTypes.Length - _activeEffects.Count
                    ))
                    .First(),
                0
            );
        }
    }

    void Awake()
    {
        _activeEnigmaAreas = new HashSet<int>();
        _activeEffects = new HashSet<TileType>();
    }

    void Start()
    {
        if (!_initd) throw new System.NotImplementedException();
    }

    void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null) DoHit(enemy);
    }

    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f)) return;

        if (_enemy != null) _dest = _enemyTfm.position;

        var dist = C.ProjectileDistancePerSecond
            * Time.deltaTime
            * (_activeEffects.Contains(TileType.Speed)
                ? C.ProjectileSpeedEffectMultiplier
                : 1f
            )
            * (_activeEffects.Contains(TileType.Slow)
                ? C.ProjectileSlowEffectMultiplier
                : 1f
            );

        var curPos = transform.position;
        var newPos = Vector3.MoveTowards(curPos, _dest, dist);

        if (Vector3.SqrMagnitude(newPos - _dest) <= C.ProjectileHitThreshold)
        {
            DoHit(_enemy);
            return;
        }

        transform.position = newPos;

        if (
            (!_haveRotated || _enemy != null) &&
            newPos != curPos
        )
        {
            var rot = Quaternion.LookRotation(
                newPos - curPos,
                Vector3.up
            );

            transform.rotation = rot;

            if (!_haveRotated)
            {
                _haveRotated = true;

                Instantiate(
                    C.ParticleShootHit,
                    transform.position,
                    rot
                );
            }
        }
    }

    void OnDestroy()
    {
        if (GameController.IsGameOver) return;

        if (_singleParticles.particleCount != 0)
        {
            _singleParticles.transform.parent = null;

            var main = _singleParticles.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }
    }

    void DoHit(Enemy enemy, bool isSpread = false)
    {
        if (
            !isSpread &&
            _activeEffects.Contains(TileType.Spread)
        )
        {
            var pos = transform.position;
            Enemy closest = null;
            var closestPos = pos;
            var closestDist = float.MaxValue;

            foreach (var e2 in Enemy.Enemies)
            {
                if (e2 == enemy) continue;

                var e2Pos = e2.transform.position;
                var dist = Vector3.SqrMagnitude(pos - e2Pos);
                if (dist > closestDist) continue;

                closest = e2;
                closestPos = e2Pos;
                closestDist = dist;
            }

            if (
                closest != null &&
                closestDist <= C.ProjectileSpreadEffectMaxDistSq
            )
            {
                Instantiate(C.SpreadPointer)
                    .GetComponent<SpreadPointer>()
                    .Init(pos, closestPos);

                SoundController.ProjectileSpread();
                DoHit(closest, true);
            }
        }

        if (enemy != null)
        {
            if (enemy.Life != 1)
            {
                if (_activeEffects.Contains(TileType.Slow))
                {
                    if (!isSpread) SoundController.ProjectileSlow();
                    enemy.AddSlowCounter();
                }

                if (_activeEffects.Contains(TileType.Burn))
                {
                    if (!isSpread) SoundController.ProjectileBurn();
                    enemy.AddBurnCounter();
                }

                if (_activeEffects.Contains(TileType.Speed))
                {
                    if (!isSpread) SoundController.ProjectileSpeed();
                    ApplyKnockback(enemy);
                }
            }

            Instantiate(
                C.ParticleShootHit,
                transform.position,
                transform.rotation * _inverse
            );

            SoundController.EnemyHit();
            --enemy.Life;
        }

        if (!isSpread) Destroy(gameObject);
    }

    void ApplyKnockback(Enemy enemy)
    {
        var enemyPos = enemy.transform.position;

        enemy.transform.position = enemyPos +
            (enemyPos - transform.position).normalized *
                C.ProjectileSpeedEffectKnockbackMagnitude;
    }
}
