using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TA
{
    static HashSet<TileType> _allowedGenerationAreas = new HashSet<TileType>
    {   TileType.Speed
    ,   TileType.Slow
    ,   TileType.Burn
    ,   TileType.Spread
    ,   TileType.Enigma
    };

    static HashSet<TileAddition> _allowedGenerationAdditions = new HashSet<TileAddition>
    {   TileAddition.Collider
    };

    static Vector2Int[] _dirs = new Vector2Int[]
    {   new Vector2Int(0, 1)
    ,   new Vector2Int(1, 0)
    ,   new Vector2Int(-1, 0)
    ,   new Vector2Int(0, -1)
    };

    static FakeTile _tile = new FakeTile();
    static FakeTile _tileWithAddition = new FakeTile { HasAdditions = true };

    public static int RowColToN(int width, int row, int col) =>
        row * (width + 1) + col;

    public static Vector2Int NToRowCol(int width, int n) =>
        new Vector2Int(
            n % (width + 1),
            Mathf.FloorToInt((float)n / (width + 1))
        );
    
    public static bool CheckGenerationValidity(GenerationData generation)
    {
        var width = generation.Width;
        var height = generation.Height;

        var map = new Dictionary<int, ITile>();

        map[RowColToN(width, height / 2, 0)] = _tileWithAddition;
        map[RowColToN(width, height / 2, width - 1)] = _tileWithAddition;

        // Basic validation
        if (
            generation.EnemiesToSpawn < 0 ||
            width < 3 ||
            width % 2 != 1 ||
            height < 3 ||
            height % 2 != 1
        ) return false;

        if (generation.Areas != null)
        {
            if (
                generation.Areas.Any(_ =>
                    !_allowedGenerationAreas.Contains(_.Type)
                )
            ) return false;

            foreach (var area in generation.Areas)
            {
                var x = area.X;
                var y = area.Y;

                // Ensure valid coordinates
                if (
                    x < 1 ||
                    x >= width - 1 ||
                    y < 1 ||
                    y >= height - 1
                ) return false;

                // Only attempt allowed areas
                if (!_allowedGenerationAreas.Contains(area.Type))
                    return false;

                var minRow = y - 1;
                var maxRow = y + 1;
                var minCol = x - 1;
                var maxCol = x + 1;
                for (var testY = minRow; testY <= maxRow; ++testY)
                {
                    for (var testX = minCol; testX <= maxCol; ++testX)
                    {
                        // Don't touch the start/end points
                        // Don't overlap any areas
                        var n = RowColToN(width, testY, testX);
                        if (map.ContainsKey(n)) return false;

                        map[n] = _tile;
                    }
                }
            }
        }

        if (generation.TileAdditions != null)
        {
            if (
                generation.TileAdditions.Any(_ =>
                    !_allowedGenerationAdditions.Contains(_.Addition)
                )
            ) return false;

            foreach (var addition in generation.TileAdditions)
            {
                var x = addition.X;
                var y = addition.Y;

                // Ensure valid coordinates
                if (
                    x < 0 ||
                    x >= width ||
                    y < 0 ||
                    y >= height
                ) return false;

                // Only attempt allowed additions
                if (!_allowedGenerationAdditions.Contains(addition.Addition))
                    return false;

                // Don't touch the start/end points
                // Don't overlap any additions
                var n = RowColToN(width, y, x);
                if (
                    map.ContainsKey(n) &&
                    map[n].HasAdditions
                ) return false;

                map[n] = _tileWithAddition;
            }
        }

        return CanPath(width, height, map);
    }
    
    public static GenerationData Generate(int seed, int width, int height)
    {
        if (
            width == 1 ||
            width % 2 != 1 ||
            height == 1 ||
            height % 2 != 1
        ) throw new NotSupportedException();

        var map = new Dictionary<int, ITile>();

        map[RowColToN(width, height / 2, 0)] = _tileWithAddition;
        map[RowColToN(width, height / 2, width - 1)] = _tileWithAddition;

        UnityEngine.Random.InitState(seed);

        // Place random areas
        var areas = new List<GenerationAreaData>();
        {
            var types = new TileType[]
            {   TileType.Speed
            ,   TileType.Slow
            ,   TileType.Burn
            ,   TileType.Spread
            ,   TileType.Enigma
            };

            var maxPlacementAttempts = 50;
            var initialChance = 0.98f;

            var size = width * height;
            var consecutiveChanceMultiplier = (float)size / (15 + size);

            var curChance = initialChance;
            while (UnityEngine.Random.value < curChance)
            {
                var type = types[UnityEngine.Random.Range(0, types.Length)];

                var row = 0;
                var col = 0;
                var canPlace = false;

                for (var attempt = 0; attempt != maxPlacementAttempts; ++attempt)
                {
                    row = UnityEngine.Random.Range(1, height - 1);
                    col = UnityEngine.Random.Range(1, width - 1);

                    var attemptIsValid = true;

                    var minRow = row - 1;
                    var maxRow = row + 1;
                    var minCol = col - 1;
                    var maxCol = col + 1;
                    for (var y = minRow; y <= maxRow; ++y)
                    {
                        for (var x = minCol; x <= maxCol; ++x)
                        {
                            if (map.ContainsKey(RowColToN(width, y, x)))
                            {
                                attemptIsValid = false;
                                break;
                            }
                        }
                    }

                    if (attemptIsValid)
                    {
                        canPlace = true;
                        break;
                    }
                }

                if (!canPlace) break;

                {
                    var minRow = row - 1;
                    var maxRow = row + 1;
                    var minCol = col - 1;
                    var maxCol = col + 1;
                    for (var y = minRow; y <= maxRow; ++y)
                        for (var x = minCol; x <= maxCol; ++x)
                            map[RowColToN(width, y, x)] = _tile;
                }

                areas.Add(new GenerationAreaData(col, row, type));

                curChance *= consecutiveChanceMultiplier;
            }
        }

        // Place random colliders
        var colliders = new List<GenerationTileAdditionData>();
        {
            var maxPlacementAttempts = 10;

            var size = width * height;
            var consecutiveChanceMultiplier = (float)size / (0.5f + size);

            var curChance = consecutiveChanceMultiplier;

            while (UnityEngine.Random.value < curChance)
            {
                var row = 0;
                var col = 0;
                var n = 0;

                var canPlace = false;

                for (var attempt = 0; attempt != maxPlacementAttempts; ++attempt)
                {
                    row = UnityEngine.Random.Range(0, height);
                    col = UnityEngine.Random.Range(0, width);
                    n = RowColToN(width, row, col);

                    if (
                        !map.ContainsKey(n) ||
                        !map[n].HasAdditions
                    )
                    {
                        canPlace = true;
                        break;
                    }
                }

                if (!(
                    canPlace &&
                    CanPath(
                        width, height, map,
                        new Vector2Int(col, row)
                    )
                )) break;

                colliders.Add(new GenerationTileAdditionData(col, row, TileAddition.Collider));
                map[n] = _tileWithAddition;

                curChance *= consecutiveChanceMultiplier;
            }
        }

        return new GenerationData(
            0,
            width,
            height,
            areas,
            colliders
        );
    }
    
    public static bool CanPath(
        int width,
        int height,
        IReadOnlyDictionary<int, ITile> map,
        Vector2Int? attemptedPlacement = null
    )
    {
        var reached = new HashSet<int>();
        var extents = new Queue<int>();

        var midRow = height / 2;
        var startN = RowColToN(width, midRow, width - 1);
        var destN = RowColToN(width, midRow, 0);

        reached.Add(startN);
        extents.Enqueue(startN);

        while (extents.Count != 0)
        {
            var n = extents.Dequeue();
            var cur = NToRowCol(width, n);

            foreach (var dir in _dirs)
            {
                var newPos = cur + dir;
                var newN = RowColToN(width, newPos.y, newPos.x);

                if (
                    reached.Contains(newN) ||
                    newPos.x >= width ||
                    newPos.x < 0 ||
                    newPos.y >= height ||
                    newPos.y < 0 ||
                    (
                        attemptedPlacement.HasValue &&
                        newPos == attemptedPlacement
                    ) ||
                    (
                        map.ContainsKey(newN) &&
                        map[newN].HasAdditions &&
                        newN != destN
                    )
                ) continue;

                if (newN == destN) return true;

                reached.Add(newN);
                extents.Enqueue(newN);
            }
        }

        return false;
    }

    class FakeTile : ITile
    {
        public bool HasAdditions { get; set; }
    }
}

public interface ITile
{
    bool HasAdditions { get; }
}

[Serializable]
public class GenerationData
{
    public int EnemiesToSpawn => enemiesToSpawn;
    public int Width => width;
    public int Height => height;
    public IReadOnlyList<GenerationAreaData> Areas => areas;
    public IReadOnlyList<GenerationTileAdditionData> TileAdditions => tileAdditions;
    public int Tutorial => tutorial;

    #pragma warning disable CS0649
    [SerializeField] int enemiesToSpawn;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] List<GenerationAreaData> areas;
    [SerializeField] List<GenerationTileAdditionData> tileAdditions;
    [SerializeField] int tutorial;
    #pragma warning restore CS0649

    public GenerationData(
        int enemiesToSpawn,
        int width,
        int height,
        List<GenerationAreaData> areas,
        List<GenerationTileAdditionData> tileAdditions
    )
    {
        this.enemiesToSpawn = enemiesToSpawn;
        this.width = width;
        this.height = height;
        this.areas = areas;
        this.tileAdditions = tileAdditions;
    }
}

[Serializable]
public struct GenerationTileAdditionData
{
    public int X => x;
    public int Y => y;
    public TileAddition Addition => addition;

    #pragma warning disable CS0649
    [SerializeField] int x;
    [SerializeField] int y;
    [SerializeField] TileAddition addition;
    #pragma warning restore CS0649

    public GenerationTileAdditionData(int x, int y, TileAddition addition)
    {
        this.x = x;
        this.y = y;
        this.addition = addition;
    }
}

[Serializable]
public struct GenerationAreaData
{
    public int X => x;
    public int Y => y;
    public TileType Type => type;

    #pragma warning disable CS0649
    [SerializeField] int x;
    [SerializeField] int y;
    [SerializeField] TileType type;
    #pragma warning restore CS0649

    public GenerationAreaData(int x, int y, TileType type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }
}