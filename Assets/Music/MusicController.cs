using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{
    public static bool MusicOn { get; private set; }

    static MusicController _i;

    [RuntimeInitializeOnLoadMethod()]
    static void Prepare()
    {
        MusicOn = PlayerPrefs.GetInt(C.PPMusicOff, 0) == 0;

        DontDestroyOnLoad(Instantiate(Resources.Load("MusicController")));
    }

    public static void Toggle()
    {
        MusicOn = !MusicOn;
        PlayerPrefs.SetInt(C.PPMusicOff, MusicOn ? 0 : 1);

        _i.SetOnOff();
    }

    #pragma warning disable CS0649
    [SerializeField] AudioClip[] _tracks;
    [SerializeField] AudioMixer _mixer;
    #pragma warning restore CS0649

    AudioSource _source;
    AudioClip[] _shuffledTracks;
    int _curTrack;

    void Awake()
    {
        _source = GetComponent<AudioSource>();

        _shuffledTracks = _tracks
            .OrderBy(_ => UnityEngine.Random.value)
            .ToArray();
        
        StartCoroutine(LoopThroughTracks());
    }

    void OnEnable() => _i = this;

    void Start() => SetOnOff();

    void SetOnOff() =>
        _mixer.SetFloat("volume", MusicOn ? 0 : -100f);

    IEnumerator LoopThroughTracks()
    {
        while (true)
        {
            var track = _shuffledTracks[_curTrack];

            _source.clip = track;

            _source.Stop();
            _source.Play();

            yield return new WaitForSecondsRealtime(track.length);

            ++_curTrack;
            if (_curTrack == _shuffledTracks.Length) _curTrack = 0;
        }
    }
}
