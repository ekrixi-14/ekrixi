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
        var activeRules = QueryActiveRules();
        while (activeRules.MoveNext(out _, out var component, out _))
        {
            if (component.Generated)
                return;

            if (!_configurationManager.GetCVar(CCVars.GenerateFTLPointsRoundstart))
                return;
            var station = _pointsSystem.GenerateSector(20);

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
}
