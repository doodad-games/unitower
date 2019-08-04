using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    public static bool SoundOn { get; private set; }

    static SoundController _i;

    [RuntimeInitializeOnLoadMethod()]
    static void Prepare()
    {
        SoundOn = PlayerPrefs.GetInt(C.PPSoundOff, 0) == 0;

        DontDestroyOnLoad(Instantiate(Resources.Load("SoundController")));
    }

    public static void Toggle()
    {
        SoundOn = !SoundOn;
        PlayerPrefs.SetInt(C.PPSoundOff, SoundOn ? 0 : 1);

        _i.SetOnOff();
    }
    
    static void Play(AudioSource audio)
    {
        audio.Stop();
        audio.Play();
    }

    #pragma warning disable CS0649
    public static void Click() => Play(_i._click);
    [SerializeField] AudioSource _click;

    public static void Close() => Play(_i._close);
    [SerializeField] AudioSource _close;

    public static void EnemyBurn() => Play(_i._enemyBurn);
    [SerializeField] AudioSource _enemyBurn;

    public static void EnemyDie() => Play(_i._enemyDie);
    [SerializeField] AudioSource _enemyDie;

    public static void EnemyHit() => Play(_i._enemyHit);
    [SerializeField] AudioSource _enemyHit;

    public static void EnemySlow() => Play(_i._enemySlow);
    [SerializeField] AudioSource _enemySlow;

    public static void EnemySpawn() => Play(_i._enemySpawn);
    [SerializeField] AudioSource _enemySpawn;

    public static void EnemySpeed() => Play(_i._enemySpeed);
    [SerializeField] AudioSource _enemySpeed;

    public static void Error() => Play(_i._error);
    [SerializeField] AudioSource _error;

    public static void Lose() => Play(_i._lose);
    [SerializeField] AudioSource _lose;

    public static void Hover() => Play(_i._hover);
    [SerializeField] AudioSource _hover;

    public static void ProjectileBurn() => Play(_i._projectileBurn);
    [SerializeField] AudioSource _projectileBurn;

    public static void ProjectileSlow() => Play(_i._projectileSlow);
    [SerializeField] AudioSource _projectileSlow;

    public static void ProjectileSpeed() => Play(_i._projectileSpeed);
    [SerializeField] AudioSource _projectileSpeed;

    public static void ProjectileSpread() => Play(_i._projectileSpread);
    [SerializeField] AudioSource _projectileSpread;

    public static void Shoot() => Play(_i._shoot);
    [SerializeField] AudioSource _shoot;

    public static void TowerAvailable() => Play(_i._towerAvailable);
    [SerializeField] AudioSource _towerAvailable;

    public static void TowerEnigmaPlaced() => Play(_i._towerEnigmaPlaced);
    [SerializeField] AudioSource _towerEnigmaPlaced;

    public static void TowerNonePlaced() => Play(_i._towerNonePlaced);
    [SerializeField] AudioSource _towerNonePlaced;

    public static void TowerSlowPlaced() => Play(_i._towerSlowPlaced);
    [SerializeField] AudioSource _towerSlowPlaced;

    public static void TowerSpeedPlaced() => Play(_i._towerSpeedPlaced);
    [SerializeField] AudioSource _towerSpeedPlaced;

    public static void TowerSpreadPlaced() => Play(_i._towerSpreadPlaced);
    [SerializeField] AudioSource _towerSpreadPlaced;

    public static void Win() => Play(_i._win);
    [SerializeField] AudioSource _win;

    [SerializeField] AudioMixer _mixer;
    #pragma warning restore CS0649

    void OnEnable() => _i = this;

    void Start() => SetOnOff();

    void SetOnOff() =>
        _mixer.SetFloat("volume", SoundOn ? 0 : -100f);
}
