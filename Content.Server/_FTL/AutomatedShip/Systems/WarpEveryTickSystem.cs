using Content.Server._FTL.AutomatedShip.Components;
using Content.Server._FTL.FTLPoints.Systems;
using Content.Server._FTL.FTLPoints.Tick;
using Content.Server._FTL.FTLPoints.Tick.AvoidStar;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server._FTL.AutomatedShip.Systems;

public sealed class WarpEveryTickSystem : StarmapTickSystem<AutomatedShipComponent>
{
    [Dependency] private readonly FtlPointsSystem _ftlPointsSystem = default!;
    [Dependency] private readonly ShuttleSystem _shuttleSystem = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    protected override void Ticked(EntityUid uid, AutomatedShipComponent component, float frameTime)
    {
        base.Ticked(uid, component, frameTime);

        if (component.AiState == AutomatedShipComponent.AiStates.Fighting)
            return; // can't warp mid-fight

        var star = _ftlPointsSystem.GetStarWithMapId(Transform(uid).MapID);
        if (!star.HasValue)
            return;
        var stars = _ftlPointsSystem.GetStarsInRange(star.Value.Position, 10);
        var destination = _random.Pick(stars);
        var mapUid = _mapManager.GetMapEntityId(destination.Map);

        if (HasComp<AvoidStarComponent>(mapUid))
            return;

        var shuttleComp = EnsureComp<ShuttleComponent>(uid);
        _shuttleSystem.FTLTravel(uid, shuttleComp, mapUid);
    }
}
