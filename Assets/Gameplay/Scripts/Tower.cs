using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class Tower : MonoBehaviour
{
    const int NumParticles = 4;
    const float KnockbackDistance = 0.1f;
    const float KnockbackDuration = 0.3f;

    static TileType[] _enigmaticTypes = new TileType[]
    {   TileType.Slow
    ,   TileType.Speed
    ,   TileType.Spread
    };

    #pragma warning disable CS0649
    [SerializeField] ParticleSystem _particles;
    [SerializeField] Transform _knockbackOffset;
    #pragma warning restore CS0649

    TextMeshPro _text;

    bool _initd;

    TileType _activeType;
    Enemy _target;
    bool _shooting;
    Vector3 _knockedBackTo;
    float _knockbackStartedAt;
    float _knockbackEndAt;

    public void Init(Tile tile)
    {
        if (
            _initd ||
            tile.Type == TileType.Burn
        ) throw new System.NotSupportedException();
        _initd = true;

        if (tile.Type != TileType.None)
            _particles.Emit(NumParticles);

        if (tile.Type == TileType.Enigma)
            StartCoroutine(DoEnigma());
        else
            SetActiveType(tile.Type);
        
        if (tile.Type == TileType.Enigma) SoundController.TowerEnigmaPlaced();
        if (tile.Type == TileType.None) SoundController.TowerNonePlaced();
        if (tile.Type == TileType.Slow) SoundController.TowerSlowPlaced();
        if (tile.Type == TileType.Speed) SoundController.TowerSpeedPlaced();
        if (tile.Type == TileType.Spread) SoundController.TowerSpreadPlaced();
    }

    void Awake() =>
        _text = GetComponent<TextMeshPro>();

    void OnEnable()
    {
        Spawner.MaybeStartSpawning();

        Enemy.onNewEnemy += NewEnemyAdded;
        CheckForClosestTarget();
    }

    void Start()
    {
        if (!_initd) throw new System.NotSupportedException();
    }

    void Update()
    {
        if (_knockbackStartedAt != 0f)
        {
            if (_knockbackEndAt < Time.time)
            {
                _knockbackStartedAt = 0f;
                _knockbackEndAt = 0f;

                _knockbackOffset.localPosition = Vector3.zero;
            }
            else
                _knockbackOffset.localPosition = Vector3.Lerp(
                    _knockedBackTo,
                    Vector3.zero,
                    (Time.time - _knockbackStartedAt) / 
                        (_knockbackEndAt - _knockbackStartedAt)
                );
        }
    }

    void OnDisable()
    {
        if (_target != null) _target.onDisable -= ClearTarget;
        Enemy.onNewEnemy -= NewEnemyAdded;
    }

    void CheckForClosestTarget()
    {
        var pos = transform.position;
        Enemy closest = null;
        var closestDist = float.MaxValue;

        foreach (var enemy in Enemy.Enemies)
        {
            var dist = Vector3.SqrMagnitude(pos - enemy.transform.position);
            if (dist > closestDist) continue;

            closest = enemy;
            closestDist = dist;
        }
        
        if (closest != null) SetUpNewTarget(closest);
    }

    void NewEnemyAdded(Enemy newEnemy)
    {
        if (_target == null) SetUpNewTarget(newEnemy);
    }

    void SetUpNewTarget(Enemy newEnemy)
    {
        _target = newEnemy;
        _target.onDisable += ClearTarget;

        if (!_shooting) StartCoroutine(StartShooting());
    }

    void ClearTarget()
    {
        _target.onDisable -= ClearTarget;
        _target = null;

        CheckForClosestTarget();
    }

    void SetActiveType(TileType type)
    {
        _activeType = type;

        var colour = C.TowerParticleColour[type];

        _text.color = colour;

        var particles = new ParticleSystem.Particle[NumParticles];
        _particles.GetParticles(particles);

        for (var i = 0; i != NumParticles; ++i)
            particles[i].startColor = colour;

        _particles.SetParticles(particles);
    }

    IEnumerator DoEnigma()
    {
        while (true)
        {
            SetActiveType(_enigmaticTypes[
                UnityEngine.Random.Range(0, _enigmaticTypes.Length)
            ]);
            
            yield return new WaitForSeconds(C.TowerEnigmaEffectElementDuration);
        }
    }

    IEnumerator StartShooting()
    {
        _shooting = true;

        while (true)
        {
            if (_target == null) break;

            Shoot();

            var speedMult = 
                (_activeType == TileType.Speed
                    ? C.TowerSpeedEffectCooldownMultiplier
                    : 1f
                ) *
                (_activeType == TileType.Slow
                    ? C.TowerSlowEffectCooldownMultiplier
                    : 1f
                );

            var xz =
                (transform.position - _target.transform.position)
                    .normalized * KnockbackDistance;
            _knockedBackTo = new Vector3(xz.x, xz.z, 0f);
            _knockbackOffset.localPosition = _knockedBackTo;
            
            _knockbackStartedAt = Time.time;
            _knockbackEndAt = Time.time + KnockbackDuration * speedMult;

            yield return new WaitForSeconds(
                C.TowerCooldownBase *
                    UnityEngine.Random.Range(
                        C.TowerCooldownRandomMin,
                        C.TowerCooldownRandomMax
                    ) * speedMult
            );
        }

        _shooting = false;
    }

    void Shoot()
    {
        Shoot(_target);

        if (
            _activeType == TileType.Spread &&
            Enemy.Enemies.Count > 1
        )
            Shoot(
                Enemy.Enemies
                    .Where(_ => _ != _target)
                    .Skip(UnityEngine.Random.Range(0, Enemy.Enemies.Count - 2))
                    .First()
            );
    }

    void Shoot(Enemy enemy)
    {
        Instantiate(
            C.Projectile,
            transform.position,
            Quaternion.identity
        )
            .GetComponent<Projectile>()
            .Init(enemy);
    }
}
