using Godot;
using System.Linq;

public partial class Pathfinding : Node
{
    public AStarGrid2D _aStarGrid2D;
    private TileMap tileMap;
    private Rect2I tileMapRect;

    public static Pathfinding Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        tileMap = TurnManager.Instance.tileMap;
        tileMapRect = tileMap.GetUsedRect();
        var mapSize = tileMapRect.End - tileMapRect.Position;
        var mapRegion = new Rect2I(tileMapRect.Position, mapSize);
        _aStarGrid2D = new AStarGrid2D()
        {
            Region = mapRegion, CellSize = tileMap.TileSet.TileSize,
            DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan,
            DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never,
            DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan
        };
        _aStarGrid2D.Update();
        SetupWalkableTiles();
    }

    private TileData TileCellAtPosition(Vector2I pos)
    {
        return tileMap.GetCellTileData(1, pos);
    }

    private bool IsTileWalkable(Vector2I pos)
    {
        return TileCellAtPosition(pos) != null;
    }

    public void SetupWalkableTiles()
    {
        for (int x = tileMapRect.Position.X; x < tileMapRect.End.X; x++)
        {
            for (int y = tileMapRect.Position.Y; y < tileMapRect.End.Y; y++)
            {
                var localPos = new Vector2I(x, y);

                _aStarGrid2D.SetPointSolid(localPos, IsTileWalkable(localPos));
            }
        }

        for (int i = 0; i < TurnManager.Instance.allActors.Count; i++)
        {
            var allActorCurrentPos =
                TurnManager.Instance.GetTilePosition(TurnManager.Instance.allActors[i].GlobalPosition);
            _aStarGrid2D.SetPointSolid(allActorCurrentPos, true);
        }
    }

    public Vector2[] FindPath(Vector2 from, Vector2 to)
    {
        var localFrom = tileMap.LocalToMap(tileMap.ToLocal(from));
        var localTo = tileMap.LocalToMap(tileMap.ToLocal(to));

        if (_aStarGrid2D.IsInBounds(localFrom.X, localFrom.Y) == false ||
            _aStarGrid2D.IsInBounds(localTo.X, localTo.Y) == false)
        {
            return [Vector2.Zero];
        }

        var path = _aStarGrid2D.GetIdPath(localFrom, localTo);

        if (path.Count == 0)
        {
            return [Vector2.Zero];
        }

        Vector2[] goodPath = new Vector2[path.Count];

        for (int i = 0; i < path.Count; i++)
        {
            goodPath[i] = tileMap.MapToLocal(path[i]);
        }

        return goodPath;
    }

    public Vector2[] FindPath(Vector2I from, Vector2I to)
    {
        if (_aStarGrid2D.IsInBounds(from.X, from.Y) == false || _aStarGrid2D.IsInBounds(to.X, to.Y) == false)
        {
            return [Vector2.Zero];
        }

        var path = _aStarGrid2D.GetIdPath(from, to);
        GD.Print("Solid?: ", _aStarGrid2D.IsPointSolid(to));
        if (path.Count == 0)
        {
            return [Vector2.Zero];
        }

        Vector2[] goodPath = new Vector2[path.Count];

        for (int i = 0; i < path.Count; i++)
        {
            goodPath[i] = tileMap.MapToLocal(path[i]);
        }

        return goodPath;
    }

    public Vector2I[] FindTilePath(Vector2I from, Vector2I to)
    {
        if (!_aStarGrid2D.IsInBounds(from.X, from.Y) || !_aStarGrid2D.IsInBounds(to.X, to.Y))
        {
            return [Vector2I.Zero];
        }

        var path = _aStarGrid2D.GetIdPath(from, to);
        return path.ToArray();
    }

    public void SetTileSolid(Vector2I pos, bool isSolid, CombatActor entity)
    {
        if (entity == null) return;
       if(pos == TurnManager.Instance.GetTilePosition(entity.GlobalPosition))
        _aStarGrid2D.SetPointSolid(pos, isSolid);
    }

    public void SetTileSolid(Vector2 pos, bool isSolid, CombatActor entity)
    {
        if (entity == null) return;
       if(pos == entity.GlobalPosition)
        _aStarGrid2D.SetPointSolid(TurnManager.Instance.GetTilePosition(pos), isSolid);
    }
}