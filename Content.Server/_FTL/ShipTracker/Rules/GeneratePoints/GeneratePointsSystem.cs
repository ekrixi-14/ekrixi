using System.Linq;
using Content.Server._FTL.FTLPoints.Systems;
using Content.Server.GameTicking.Rules;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Robust.Shared.Configuration;
using Content.Shared.CCVar;
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

        SubscribeLocalEvent<PlayerSpawningEvent>(OnPlayerSpawnEvent);
    }

    private void OnPlayerSpawnEvent(PlayerSpawningEvent ev)
    {
        var component = EntityQuery<GeneratePointsComponent>().First();

        if (component.Generated)
            return;

        if (!_configurationManager.GetCVar(CCVars.GenerateFTLPointsRoundstart))
            return;
        // TODO make work??
        var station = _pointsSystem.GenerateSector(40);

        if (ev.Station.HasValue)
        {
            if (TryComp<StationDataComponent>(ev.Station.Value, out var stationDataComponent))
            {
                var grid = _stationSystem.GetLargestGrid(stationDataComponent);
                if (grid.HasValue)
                {
                    var shuttle = EnsureComp<ShuttleComponent>(grid.Value);
                    _shuttleSystem.FTLTravel(grid.Value, shuttle, _mapManager.GetMapEntityId(station));
                }
            }
        }

        component.Generated = true;

        Log.Info("Finished generation of sector.");
    }
}
