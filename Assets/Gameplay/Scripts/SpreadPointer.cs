using UnityEngine;

public class SpreadPointer : MonoBehaviour
{
    const float WidthMultiple = 0.15f;
    const float Shrinkage = 0.5f;
    const float HeadPadding = 0.1f;

    #pragma warning disable CS0649
    [SerializeField] SpriteRenderer _dots;
    [SerializeField] Transform _head;
    #pragma warning restore CS0649

    bool _initd;

    public void Init(Vector3 from, Vector3 to)
    {
        if (_initd) throw new System.NotSupportedException();
        _initd = true;

        var dir = (to - from).normalized;
        var dist = Mathf.Max(Vector3.Distance(from, to) - Shrinkage, 0f);

        var length = Mathf.Floor(dist / WidthMultiple) * WidthMultiple;

        var size = _dots.size;
        _dots.size = new Vector2(length, size.y);

        _head.localPosition = new Vector3(
            dist == 0f
                ? 0f
                : length / 2 + HeadPadding,
            0f, 0f
        );

        transform.position = from + dir * dist * 0.5f;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.forward);
    }

    public void Destroy() => GameObject.Destroy(gameObject);

    void Start()
    {
        if (!_initd) throw new System.NotSupportedException();
    }
}
