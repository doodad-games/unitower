using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, ITile
{
    public bool HasAdditions { get; private set; }

    #pragma warning disable CS0649
    [SerializeField] Transform _container;
    [SerializeField] SpriteRenderer _renderer;
    #pragma warning restore CS0649

    public Vector2Int Coord => _coord;
    public TileType Type => _type;

    Vector2Int _coord;

    bool _initd;
    int _area;
    TileType _type;

    public void Init(TileType type, int area)
    {
        if (
            _initd ||
            (
                type == TileType.Enigma &&
                area == 0
            )
        ) throw new System.NotSupportedException();
        _initd = true;

        _type = type;
        _area = area;
        _renderer.color = C.TileColour[_type];
    }

    public void Add(TileAddition addition)
    {
        if (addition == TileAddition.None) throw new System.NotSupportedException();

        HasAdditions = true;

        if (addition == TileAddition.StartEndPointer)
            Instantiate(C.TileStartEndPointer, _container);
        if (addition == TileAddition.Collider)
            Instantiate(C.TileCollider, _container);
        if (addition == TileAddition.Tower)
            Instantiate(C.TileTower, _container)
                .GetComponentInChildren<Tower>()
                .Init(this);
    }

    void Awake()
    {
        var pos = transform.position;
        _coord = new Vector2Int(
            Mathf.RoundToInt(pos.x),
            Mathf.RoundToInt(pos.z)
        );
    }

    void Start()
    {
        if (!_initd) throw new System.NotSupportedException();
    }

    void OnTriggerEnter(Collider other)
    {
        if (_type == TileType.None) return;

        var projectile = other.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.AddEffect(_type, _area);
            return;
        }

        var enemy = other.GetComponent<Enemy>();
        if (enemy != null)
            enemy.IncrementEffectCounter(_type, _area);
    }

    void OnTriggerExit(Collider other)
    {
        if (_type == TileType.None) return;

        var enemy = other.GetComponent<Enemy>();
        if (enemy != null)
            enemy.DecrementEffectCounter(_type, _area);
    }

    void OnMouseUpAsButton()
    {
        if (
            GameController.IsGameOver ||
            UIHelper.OverUI()
        ) return;

        var pos = transform.position;
        TileController.TryPlaceTower(this);
    }
}

public enum TileType
{
    None = 0,
    Speed = 1,
    Slow = 2,
    Burn = 3,
    Spread = 4,
    Enigma = 5
}

public enum TileAddition
{
    None = 0,
    StartEndPointer = 1,
    Collider = 2,
    Tower = 3
}