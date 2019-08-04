using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static float[] _speedSettings = new float[] { 1f, 2f, 3f, 0f };

    public static event Action onScoreChanged;
    public static event Action onSeedChanged;
    public static event Action onTowersChanged;
    public static event Action onTowersAvailableChanged;

    public static bool IsGenerated { get; private set; }
    public static GenerationData Generation { get; private set; }

    public static bool IsGameOver { get; private set; }

    public static bool finishedSpawning;

    static int _curSpeed;
    static int _highestScore;
    static int _seed;

    static int _score;
    static int _towers;
    static int _towersAvailable;

    public static int SpeedSetting =>
        Mathf.RoundToInt(_speedSettings[_curSpeed]);

    public static int Score
    {
        get => _score;
        set {
            _score = value;
            onScoreChanged?.Invoke();

            MaybeUpdateHighestScore(value);

            if (
                Generation.EnemiesToSpawn != 0 &&
                value == Generation.EnemiesToSpawn
            )
            {
                WinGame();
                return;
            }

            if (value % C.EnemyExtraTowerEvery == 0)
            {
                SoundController.TowerAvailable();
                ++TowersAvailable;
            }
        }
    }

    public static int Seed
    {
        get => _seed;
        set {
            _seed = value;
            onSeedChanged?.Invoke();
        }
    }

    public static int Towers
    {
        get => _towers;
        set {
            _towers = value;
            onTowersChanged?.Invoke();
        }
    }

    public static int TowersAvailable
    {
        get => _towersAvailable;
        private set {
            _towersAvailable = value;
            onTowersAvailableChanged?.Invoke();
        }
    }

    [RuntimeInitializeOnLoadMethod()]
    static void Prepare()
    {
        Application.targetFrameRate = 60;

        _highestScore = PlayerPrefs.GetInt(C.PPHighestScore, 0);

        _curSpeed = PlayerPrefs.GetInt(C.PPSpeed, 0);
        if (_curSpeed < 0 || _curSpeed >= _speedSettings.Length) _curSpeed = 0;
        ApplyCurrentSpeed();

        DontDestroyOnLoad(Instantiate(Resources.Load("Config")));
        DontDestroyOnLoad(Instantiate(Resources.Load("GameController")));

        var now = DateTime.Now.ToUniversalTime();
        UnityEngine.Random.InitState(
            now.Year * 366 +
            now.DayOfYear
        );

        ShuffleSeed();
    }

    public static void StartGame(GenerationData generation)
    {
        ClearGameData();

        IsGenerated = generation == null;

        if (generation == null)
            generation = TA.Generate(
                _seed,
                C.GenerateStandardWidth,
                C.GenerateStandardHeight
            );
        
        if (!TA.CheckGenerationValidity(generation))
            throw new NotSupportedException();
        
        Generation = generation;

        SceneManager.LoadScene("Gameplay");
    }

    public static void LoseGame()
    {
        ScreenShake.Do();
        SoundController.Lose();
        GameOver();
        LoseGamePopup.Instance.gameObject.SetActive(true);
    }

    public static void EndGame()
    {
        ApplyCurrentSpeed();

        ClearGameData();

        SceneManager.LoadScene("Menu");
    }

    public static void ShuffleSeed() => 
        Seed = UnityEngine.Random.Range(1, 999999);

    public static void ToggleSpeed()
    {
        _curSpeed = (_curSpeed + 1) % _speedSettings.Length;
        PlayerPrefs.SetInt(C.PPSpeed, _curSpeed);

        if (!IsGameOver) ApplyCurrentSpeed();
    }

    static void WinGame()
    {
        SoundController.Win();
        GameOver();
        WinGamePopup.Instance.gameObject.SetActive(true);
    }

    static void GameOver()
    {
        Time.timeScale = 0f;

        IsGameOver = true;
    }

    static void ClearGameData()
    {
        IsGenerated = false;
        IsGameOver = false;
        _score = 0;
        _towers = 0;
        _towersAvailable = C.TowersAvailableAtStart;
        finishedSpawning = false;
    }

    static void MaybeUpdateHighestScore(int score)
    {
        if (
            !IsGenerated ||
            score <= _highestScore
        ) return;

        _highestScore = score;
        PlayerPrefs.SetInt(C.PPHighestScore, score);
        PlayerPrefs.SetInt(C.PPHighestScoreSeed, _seed);
    }

    static void ApplyCurrentSpeed() =>
        Time.timeScale = _speedSettings[_curSpeed];
}
