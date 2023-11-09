using Content.Server._FTL.FTLPoints.Systems;
using Content.Server.GameTicking.Rules;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Robust.Shared.Configuration;
using Content.Shared.CCVar;
using Robust.Server.GameObjects;
using Robust.Shared.Map;

namespace Content.Server._FTL.ShipTracker.Rules.GeneratePoints;

/// <summary>
/// Generates points roundstart, see <see cref="GeneratePointsComponent"/>.
/// </summary>
public sealed class GeneratePointsSystem : GameRuleSystem<GeneratePointsComponent>
{
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    [Dependency] private readonly FtlPointsSystem _pointsSystem = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly ShuttleSystem _shuttleSystem = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlayerSpawningEvent>(OnPlayerSpawningEvent);
    }

    private void OnPlayerSpawningEvent(PlayerSpawningEvent ev)
    {
        if (_configurationManager.GetCVar(CCVars.GenerateFTLPointsRoundstart))
        {
            var station = _pointsSystem.GenerateSector(60);
            var query = AllEntityQuery<StationJobsComponent>();
            while (query.MoveNext(out var stationEntity, out _))
            {
                var grid = _stationSystem.GetOwningStation(stationEntity);
                if (!TryComp<ShuttleComponent>(grid, out var shuttle))
                    continue;
                _shuttleSystem.FTLTravel(grid.Value, shuttle, _mapManager.GetMapEntityId(station));
            }
        }
    }
}
