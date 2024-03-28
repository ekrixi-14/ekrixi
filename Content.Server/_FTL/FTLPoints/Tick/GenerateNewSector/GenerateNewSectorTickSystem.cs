using Content.Server._FTL.FTLPoints.Systems;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Robust.Shared.Map;

namespace Content.Server._FTL.FTLPoints.Tick.GenerateNewSector;

/// <summary>
/// This system spawns ships every tick.
/// </summary>
public sealed class GenerateNewSectorTickSystem : StarmapTickSystem<GenerateNewSectorTickComponent>
{
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly FtlPointsSystem _ftlPoints = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;

    protected override void Ticked(EntityUid uid, GenerateNewSectorTickComponent component, float frameTime)
    {
        base.Ticked(uid, component, frameTime);

        var stations = _stationSystem.GetStations();
        if (stations.Count <= 0)
            return;

        stations.ForEach(ent =>
        {
            if (!TryComp<StationDataComponent>(ent, out var dataComponent))
                return;

            var mainStation = _stationSystem.GetLargestGrid(dataComponent);

            if (!mainStation.HasValue)
                return;

            if (_mapManager.GetMapEntityId(Transform(mainStation.Value).MapID) == uid)
                _ftlPoints.GenerateSector(25, Transform(uid).MapID, true, true);
        });

        Log.Debug("Tried to generate new sectors");
    }
}
