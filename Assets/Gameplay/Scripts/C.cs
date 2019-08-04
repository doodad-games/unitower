using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class C : MonoBehaviour
{
    public const string PPHighestScore = "highestScore";
    public const string PPHighestScoreSeed = "highestScoreSeed";
    public const string PPMusicOff = "musicOff";
    public const string PPSoundOff = "soundOff";
    public const string PPSpeed = "speed";

    #pragma warning disable CS0649
    public static float CameraShakeDuration => 0.3f;
    public static float CameraShakeMagnitude => 0.05f;

    public static GameObject Enemy => _i._enemyPrefab;
    [SerializeField] GameObject _enemyPrefab;

    public static float EnemySpawnRateInitial => 2.5f;
    public static float EnemySpawnRateMultiple => 0.95f;
    public static int EnemySpawnRateMultipleEvery => 7;
    public static float EnemySpawnRateMin => 1f;

    public static float EnemySpeedInitial => 0.9f;
    public static float EnemySpeedIncrementAmount => 0.05f;
    public static int EnemySpeedIncrementEvery => 8;
    public static int EnemySpeedEffectSpeedMultiplier => 2;

    public static int EnemyLifeInitial => 2;
    public static int EnemyLifeIncrementAmount => 1;
    public static int EnemyLifeIncrementEvery => 6;

    public static int EnemyExtraTowerEvery => 5;

    public static int EnemySlowEffectMaxApplications => 3;
    public static float EnemySlowEffectDuration => 1.75f;
    public static float EnemySlowEffectSpeedMultiplier => 0.75f;
    public static float EnemyBurnMovementEffectTickRate => 1f;
    public static float EnemyBurnProjectileEffectDuration => 2.1f;
    public static float EnemyBurnProjectileEffectTickRate => 1f;

    public static int GenerateStandardWidth => 17;
    public static int GenerateStandardHeight => 9;

    public static GameObject ParticleShootHit => _i._particleShootHitPrefab;
    [SerializeField] GameObject _particleShootHitPrefab;

    public static GameObject Projectile => _i._projectilePrefab;
    [SerializeField] GameObject _projectilePrefab;

    public static float ProjectileDistancePerSecond => 4.5f;
    public static float ProjectileHitThreshold => 0.0125f;

    public static float ProjectileSpeedEffectMultiplier => 1.5f;
    public static float ProjectileSpeedEffectKnockbackMagnitude => 0.1f;
    public static float ProjectileSlowEffectMultiplier => 0.66666f;
    public static float ProjectileSpreadEffectMaxDistSq => 6.5f;

    public static GameObject SpreadPointer => _i._spreadPointerPrefab;
    [SerializeField] GameObject _spreadPointerPrefab;

    public static GameObject Tile => _i._tilePrefab;
    [SerializeField] GameObject _tilePrefab;
    public static GameObject TileStartEndPointer => _i._tileStartEndPointerPrefab;
    [SerializeField] GameObject _tileStartEndPointerPrefab;
    public static GameObject TileCollider => _i._tileColliderPrefab;
    [SerializeField] GameObject _tileColliderPrefab;
    public static GameObject TileTower => _i._tileTowerPrefab;
    [SerializeField] GameObject _tileTowerPrefab;

    public static IReadOnlyDictionary<TileType, Color> TileColour
        { get; private set; }
    [SerializeField] TileTypeColour[] _tileColours;

    public static float TowerCooldownBase => 3f;
    public static float TowerCooldownRandomMin => 0.9f;
    public static float TowerCooldownRandomMax => 1.1f;

    public static float TowerSpeedEffectCooldownMultiplier => 0.75f;
    public static float TowerSlowEffectCooldownMultiplier => 1.5f;
    public static float TowerEnigmaEffectElementDuration => 4.5f;

    public static IReadOnlyDictionary<TileType, Color> TowerParticleColour
        { get; private set; }
    [SerializeField] TileTypeColour[] _towerParticleColours;

    public static int TowersAvailableAtStart => 5;
    #pragma warning restore CS0649

    static C _i;

    void OnEnable()
    {
        _i = this;

        TileColour = _tileColours.Aggregate(
            new Dictionary<TileType, Color>(),
            (dict, typeColour) => {
                dict[typeColour.type] = typeColour.colour;
                return dict;
            }
        );

        TowerParticleColour = _towerParticleColours.Aggregate(
            new Dictionary<TileType, Color>(),
            (dict, typeColour) => {
                dict[typeColour.type] = typeColour.colour;
                return dict;
            }
        );
    }

    [Serializable]
    struct TileTypeColour
    {
        #pragma warning disable CS0649
        public TileType type;
        public Color colour;
        #pragma warning restore CS0649
    }
}