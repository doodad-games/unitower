using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    static HashSet<ScreenShake> _is = new HashSet<ScreenShake>();

    public static void Do()
    {
        foreach (var i in _is)
            i._shakeUntil = Time.unscaledTime + C.CameraShakeDuration;
    }

    #pragma warning disable CS0649
    [SerializeField] Transform _reference;
    [SerializeField] float _magnitude;
    #pragma warning restore CS0649

    float _shakeUntil;

    void OnEnable() => _is.Add(this);

    void Update()
    {
        if (_shakeUntil != 0f)
        {
            if (_shakeUntil > Time.unscaledTime)
            {
                var rnd = UnityEngine.Random.insideUnitCircle *
                    C.CameraShakeMagnitude * _magnitude;

                transform.position =
                    _reference.position + new Vector3(rnd.x, 0f, rnd.y);
            }
            else
            {
                _shakeUntil = 0f;
                transform.position = _reference.position;
            }
        }
    }

    void OnDisable() => _is.Remove(this);
}
