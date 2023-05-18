using Content.Server.GameTicking.Rules.Components;
using Content.Server.StationEvents.Components;

namespace Content.Server.StationEvents.Events;

public sealed class FireGrowthRule : StationEventSystem<KudzuGrowthRuleComponent>
{
    protected override void Started(EntityUid uid, KudzuGrowthRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        // Pick a place to spark a fire.
        if (!TryFindRandomTile(out var targetTile, out _, out var targetGrid, out var targetCoords))
            return;
        Spawn("FTLFire", targetCoords);
        Sawmill.Info($"Spawning a Fire at {targetTile} on {targetGrid}");
    }
}
