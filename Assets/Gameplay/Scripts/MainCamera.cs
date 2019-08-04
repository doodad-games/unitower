using UnityEngine;

public class MainCamera : MonoBehaviour
{
    #pragma warning disable CS0649
    [SerializeField] Transform _screenShakeReference;
    #pragma warning restore CS0649

    Camera _camera;
    float _xToFit;
    float _yToFit;

    float _prevScreenWidth;
    float _prevScreenHeight;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        _xToFit = GameController.Generation.Width + 2;
        _yToFit = GameController.Generation.Height + 2;

        var newPos = new Vector3(
            (float)GameController.Generation.Width / 2,
            transform.position.y,
            GameController.Generation.Height / 2
        );

        transform.position = newPos;
        _screenShakeReference.position = newPos;
    }

    void Update()
    {
        if (
            Screen.width == _prevScreenWidth &&
            Screen.height == _prevScreenHeight
        ) return;

        _prevScreenWidth = Screen.width;
        _prevScreenHeight = Screen.height;

        var activeRatio = _prevScreenWidth / _prevScreenHeight;

        var horizontalUnitsPerSize = activeRatio * 2;

        _camera.orthographicSize = Mathf.Max(
            _xToFit / horizontalUnitsPerSize,
            _yToFit / 2f
        );
    }
}
