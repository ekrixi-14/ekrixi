using Content.Server.Maps;
using Content.Server.Shuttles.Systems;
using Content.Shared.Dataset;
using Content.Shared.Salvage;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._FTL.FTLPoints;

/// <summary>
/// This handles the generation of FTL points
/// </summary>
public sealed class FTLPointsSystem : EntitySystem
{
    [Dependency] private EntityManager _entManager = default!;
    [Dependency] private IMapManager _mapManager = default!;
    [Dependency] private IPrototypeManager _prototypeManager = default!;
    [Dependency] private MetaDataSystem _metaDataSystem = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private readonly ShuttleConsoleSystem _consoleSystem = default!;

    public const int PREFERRED_POINT_AMOUNT = 3;

    public void RegeneratePoints()
    {
        ClearPoints();

        for (int i = 0; i < PREFERRED_POINT_AMOUNT; i++)
        {
            GeneratePoint();
        }

        Log.Debug("Regenerated points.");
    }

    public void ClearPoints()
    {
        var query = EntityQueryEnumerator<DisposalFTLPointComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            DeletePoint(uid);
        }
    }

    public void DeletePoint(EntityUid point)
    {
        var xform = Transform(point);
        var map = xform.MapID;
        Del(point);
        _mapManager.DeleteMap(map);
    }

    public void GeneratePoint()
    {
        var mapId = _mapManager.CreateMap();
        Log.Debug(mapId.ToString());
        var mapUid = _mapManager.GetMapEntityId(mapId);

        var ftlUid = _entManager.CreateEntityUninitialized("FTLPoint", new EntityCoordinates(mapUid, Vector2.Zero));
        _metaDataSystem.SetEntityName(ftlUid,
            SharedSalvageSystem.GetFTLName(_prototypeManager.Index<DatasetPrototype>("names_borer"), _random.Next(0,1000000)));
        _entManager.InitializeAndStartEntity(ftlUid);
        AddComp<DisposalFTLPointComponent>(ftlUid);
        _consoleSystem.RefreshShuttleConsoles();

    }
}
