using Content.Server._FTL.FTLPoints.Systems;
using Content.Server.GameTicking.Rules;
using Content.Server.Station.Systems;
using Robust.Shared.Configuration;
using Content.Shared.CCVar;

namespace Content.Server._FTL.ShipTracker.Rules.GeneratePoints;

/// <summary>
/// Generates points roundstart, see <see cref="GeneratePointsComponent"/>.
/// </summary>
public sealed class GeneratePointsSystem : GameRuleSystem<GeneratePointsComponent>
{
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    [Dependency] private readonly FTLPointsSystem _pointsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlayerSpawningEvent>(OnPlayerSpawningEvent);
    }

    private void OnPlayerSpawningEvent(PlayerSpawningEvent ev)
    {
        if (_configurationManager.GetCVar(CCVars.GenerateFTLPointsRoundstart))
        {
            _pointsSystem.RegeneratePoints();
        }
    }
}
