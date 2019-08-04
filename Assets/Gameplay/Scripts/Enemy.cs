using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    const int NumSingleParticles = 2;

    static TileType[] _enigmaticTypes = new TileType[]
    {   TileType.Burn
    ,   TileType.Slow
    ,   TileType.Speed
    };

    public static event Action<Enemy> onNewEnemy;

    public static IReadOnlyCollection<Enemy> Enemies => _enemies;

    public event Action onDisable;

    static HashSet<Enemy> _enemies = new HashSet<Enemy>();

    #pragma warning disable CS0649
    [SerializeField] SpriteRenderer _lifeBar;
    [SerializeField] ParticleSystem _singleParticles;
    [SerializeField] ParticleSystem _persistentParticles;
    #pragma warning restore CS0649

    Dictionary<int, Tuple<int, TileType>> _effectsFromEnigma;
    Dictionary<TileType, int> _effectsFromMovement;
    int _slowParticles;
    bool _haveSpeedParticle;
    Queue<float> _slowEffectTimeouts;
    Queue<float> _burnEffectTimeouts;
    NavMeshAgent _navAgent;
    float _initialLifeBarWidth;

    bool _initd;
    bool _isDead;
    Vector3 _dest;
    int _initialLife;
    int _life;
    float _speed;

    bool _willUpdateLifeBar;

    public int Life
    {
        get => _life;
        set {
            if (_isDead) return;

            if (value <= 0)
            {
                Die();
                return;
            }

            _life = value;
            RequestUpdateLifeBar();
        }
    }

    public void Init(Vector3 dest, int life, float speed)
    {
        if (_initd) throw new NotSupportedException();
        _initd = true;

        _dest = dest;
        _navAgent.SetDestination(_dest);

        _initialLife = life;
        _life = life;
        RequestUpdateLifeBar(true);

        _speed = speed;
        _navAgent.speed = speed;

        SoundController.EnemySpawn();
    }

    public void IncrementEffectCounter(TileType type, int area)
    {
        if (
            type == TileType.None ||
            (
                type == TileType.Enigma &&
                area == 0
            )
        ) throw new NotSupportedException();

        if (type == TileType.Spread) return;

        if (type == TileType.Enigma)
        {
            if (!_effectsFromEnigma.ContainsKey(area))
            {
                var newType = _enigmaticTypes[
                    UnityEngine.Random.Range(0, _enigmaticTypes.Length)
                ];

                IncrementEffectCounter(newType, 0);
                _effectsFromEnigma[area] = Tuple.Create(1, newType);
            }
            else
            {
                var cur = _effectsFromEnigma[area];
                _effectsFromEnigma[area] =
                    Tuple.Create(cur.Item1 + 1, cur.Item2);
            }
        }
        else
        {
            ++_effectsFromMovement[type];

            if (_effectsFromMovement[type] == 1)
            {
                if (type == TileType.Speed)
                {
                    SoundController.EnemySpeed();
                    RecalculateSpeed();
                }

                if (type == TileType.Slow)
                {
                    SoundController.EnemySlow();
                    RecalculateSpeed();
                }

                if (type == TileType.Burn)
                {
                    Emit(_persistentParticles, TileType.Burn, 1);
                    StartCoroutine(BurnFromMovement());
                }
                
                Emit(_singleParticles, type, NumSingleParticles);
            }
        }
    }

    public void DecrementEffectCounter(TileType type, int area)
    {
        if (
            type == TileType.None ||
            (
                type == TileType.Enigma &&
                area == 0
            )
        ) throw new NotSupportedException();

        if (type == TileType.Spread) return;

        if (type == TileType.Enigma)
        {
            var effect = _effectsFromEnigma[area];

            if (effect.Item1 == 1)
            {
                DecrementEffectCounter(effect.Item2, 0);
                _effectsFromEnigma.Remove(area);
            }
            else
                _effectsFromEnigma[area] =
                    Tuple.Create(effect.Item1 - 1, effect.Item2);
        }
        else
        {
            --_effectsFromMovement[type];

            if (_effectsFromMovement[type] == 0)
            {
                if (
                    type == TileType.Speed ||
                    type == TileType.Slow
                ) RecalculateSpeed();

                if (type == TileType.Burn)
                    DestroyParticles(type, 1);
            }
        }
    }

    public void AddSlowCounter()
    {
        var newApplication = _slowEffectTimeouts.Count != C.EnemySlowEffectMaxApplications;

        if (!newApplication) _slowEffectTimeouts.Dequeue();
        
        _slowEffectTimeouts.Enqueue(Time.time + C.EnemySlowEffectDuration);

        if (newApplication) RecalculateSpeed();
    }

    public void AddBurnCounter()
    {
        Emit(_persistentParticles, TileType.Burn, 1);
        _burnEffectTimeouts.Enqueue(Time.time + C.EnemyBurnProjectileEffectDuration);

        if (_burnEffectTimeouts.Count == 1) StartCoroutine(BurnFromProjectiles());
    }

    void Awake()
    {
        _slowEffectTimeouts = new Queue<float>();
        _burnEffectTimeouts = new Queue<float>();

        _effectsFromEnigma = new Dictionary<int, Tuple<int, TileType>>();
        _effectsFromMovement = new Dictionary<TileType, int>
        {   { TileType.Speed, 0 }
        ,   { TileType.Slow, 0 }
        ,   { TileType.Burn, 0 }
        };

        _navAgent = GetComponent<NavMeshAgent>();

        _initialLifeBarWidth = _lifeBar.size.x;
    }

    void OnEnable()
    {
        if (_initd) _navAgent.SetDestination(_dest);

        _enemies.Add(this);
        onNewEnemy?.Invoke(this);
    }

    void Start()
    {
        if (!_initd) throw new NotSupportedException();
    }

    void Update()
    {
        var needsSpeedRecalc = false;
        while (
            _slowEffectTimeouts.Count != 0 &&
            _slowEffectTimeouts.Peek() < Time.time
        )
        {
            _slowEffectTimeouts.Dequeue();
            needsSpeedRecalc = true;
        }
        if (needsSpeedRecalc) RecalculateSpeed();

        while (
            _burnEffectTimeouts.Count != 0 &&
            _burnEffectTimeouts.Peek() < Time.time
        )
        {
            DestroyParticles(TileType.Burn, 1);
            _burnEffectTimeouts.Dequeue();
        }
    }

    void OnDisable()
    {
        _enemies.Remove(this);
        onDisable?.Invoke();
    }

    void RecalculateSpeed()
    {
        var haveSpeed = _effectsFromMovement[TileType.Speed] != 0;

        var slows = _slowEffectTimeouts.Count +
            (_effectsFromMovement[TileType.Slow] != 0 ? 1 : 0);

        _navAgent.speed = _speed *
            (haveSpeed ? C.EnemySpeedEffectSpeedMultiplier : 1) *
            (slows == 0
                ? 1
                : Mathf.Pow(C.EnemySlowEffectSpeedMultiplier, slows)
            );
        
        if (slows != _slowParticles)
        {
            if (slows > _slowParticles)
                Emit(_persistentParticles, TileType.Slow, slows - _slowParticles);
            else
                DestroyParticles(TileType.Slow, _slowParticles - slows);
            
            _slowParticles = slows;
        }

        if (haveSpeed && !_haveSpeedParticle)
        {
            _haveSpeedParticle = true;
            Emit(_persistentParticles, TileType.Speed, 1);
        }
        else if (!haveSpeed && _haveSpeedParticle)
        {
            _haveSpeedParticle = false;
            DestroyParticles(TileType.Speed, 1);
        }
    }

    IEnumerator BurnFromMovement()
    {
        while (_effectsFromMovement[TileType.Burn] != 0)
        {
            Emit(_singleParticles, TileType.Burn, 1);
            SoundController.EnemyBurn();
            --Life;

            yield return new WaitForSeconds(C.EnemyBurnMovementEffectTickRate);
        }
    }

    IEnumerator BurnFromProjectiles()
    {
        yield return new WaitForSeconds(C.EnemyBurnProjectileEffectTickRate);

        while (_burnEffectTimeouts.Count != 0)
        {
            Emit(_singleParticles, TileType.Burn, 1);
            SoundController.ProjectileBurn();

            Life -= _burnEffectTimeouts.Count;
            yield return new WaitForSeconds(C.EnemyBurnProjectileEffectTickRate);
        }
    }

    void RequestUpdateLifeBar(bool immediately = false)
    {
        if (immediately)
        {
            UpdateLifeBar();
            return;
        }

        if (_willUpdateLifeBar) return;
        
        StartCoroutine(UpdateLifeBarSoon());
    }

    IEnumerator UpdateLifeBarSoon()
    {
        yield return new WaitForEndOfFrame();
        UpdateLifeBar();
    }

    void UpdateLifeBar()
    {
        _willUpdateLifeBar = false;

        _lifeBar.size = new Vector2(
            _initialLifeBarWidth * ((float)_life / _initialLife),
            _lifeBar.size.y
        );
    }

    void Emit(ParticleSystem system, TileType type, int num)
    {
        var main = system.main;
        main.startColor = C.TowerParticleColour[type];

        system.Emit(num);
    }

    void DestroyParticles(TileType type, int num)
    {
        var tarColour = C.TowerParticleColour[type];

        var n = _persistentParticles.particleCount;
        var particles = new ParticleSystem.Particle[n];
        _persistentParticles.GetParticles(particles);

        var shot = 0;
        for (var i = 0; i != n; ++i)
        {
            if (particles[i].startColor == tarColour)
            {
                ++shot;
                particles[i].remainingLifetime = 0f;
            }

            if (shot == num) break;
        }

        _persistentParticles.SetParticles(particles);
    }

    void Die()
    {
        if (GameController.IsGameOver) return;

        if (_isDead) throw new NotSupportedException();
        _isDead = true;

        if (_singleParticles.particleCount != 0)
        {
            _singleParticles.transform.parent = null;

            var main = _singleParticles.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }

        ++GameController.Score;

        SoundController.EnemyDie();
        Destroy(gameObject);
    }
}
