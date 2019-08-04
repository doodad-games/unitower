using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    static TileController _i;

    public static void TryPlaceTower(Tile tile) =>
        _i._TryPlaceTower(tile);

    #pragma warning disable CS0649
    [SerializeField] Transform _container;
    #pragma warning restore CS0649

    int _numCols;
    Dictionary<int, Tile> _tiles;
    Dictionary<int, ITile> _iTiles;

    void Awake()
    {
        _tiles = new Dictionary<int, Tile>();
        _iTiles = new Dictionary<int, ITile>();

        var width = GameController.Generation.Width;
        var midRow = GameController.Generation.Height / 2;

        var start = CreateTile(midRow, width - 1, TileType.None);
        start.Add(TileAddition.StartEndPointer);

        var end = CreateTile(midRow, 0, TileType.None);
        end.Add(TileAddition.StartEndPointer);

        if (GameController.Generation.Areas != null)
            for (var i = 0; i != GameController.Generation.Areas.Count; ++i)
                CreateArea(i);

        for (var row = 0; row != GameController.Generation.Height; ++row)
        {
            for (var col = 0; col != GameController.Generation.Width; ++col)
            {
                if (_tiles.ContainsKey(TA.RowColToN(width, row, col))) continue;

                CreateTile(row, col, TileType.None);
            }
        }
        
        if (GameController.Generation.TileAdditions != null)
            foreach (var tileData in GameController.Generation.TileAdditions)
                _tiles[
                    TA.RowColToN(width, tileData.Y, tileData.X)
                ].Add(tileData.Addition);
    }

    void OnEnable() => _i = this;

    Tile CreateTile(int row, int col, TileType type, int area = 0)
    {
        var n = TA.RowColToN(GameController.Generation.Width, row, col);
        if (_tiles.ContainsKey(n)) throw new System.NotSupportedException();

        var tile = Instantiate(
            C.Tile,
            new Vector3(
                col,
                0f,
                row
            ),
            Quaternion.identity,
            _container
        )
            .GetComponent<Tile>();

        tile.Init(type, area);
        
        _tiles[n] = tile;
        _iTiles[n] = tile;

        return tile;
    }

    void CreateArea(int n)
    {
        var areaData = GameController.Generation.Areas[n];

        var centreRow = areaData.Y;
        var centreCol = areaData.X;

        var minRow = centreRow - 1;
        var maxRow = centreRow + 1;
        var minCol = centreCol - 1;
        var maxCol = centreCol + 1;

        for (var row = minRow; row <= maxRow; ++row)
            for (var col = minCol; col <= maxCol; ++col)
                CreateTile(row, col, areaData.Type, n + 1);
    }

    void _TryPlaceTower(Tile tile)
    {
        var coord = tile.Coord;

        if (
            tile.Type == TileType.Burn ||
            tile.HasAdditions ||
            GameController.Towers >= GameController.TowersAvailable ||
            !TA.CanPath(
                GameController.Generation.Width,
                GameController.Generation.Height,
                _iTiles,
                coord
            )
        )
        {
            ScreenShake.Do();
            SoundController.Error();
            return;
        }

        tile.Add(TileAddition.Collider);
        tile.Add(TileAddition.Tower);
        ++GameController.Towers;
    }
}
