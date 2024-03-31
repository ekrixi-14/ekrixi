using System.Linq;
using Content.Server._FTL.ShipTracker.Components;
using Content.Server._FTL.ShipTracker.Events;
using Content.Server.Chat.Systems;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Events;
using Content.Shared._FTL.ShipTracker;
using Robust.Shared.Audio;
using Robust.Shared.Map;

namespace Content.Server._FTL.ShipTracker.Systems;

/// <summary>
/// This handles tracking ships, healths and more
/// </summary>
public sealed partial class ShipTrackerSystem : SharedShipTrackerSystem
{
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        //SubscribeLocalEvent<ShipTrackerComponent, FTLStartedEvent>(OnFTLStartedEvent);
    }


    private void BroadcastToStationsOnMap(
        MapId map,
        string message,
        string sender = "Automated Ship",
        bool playDefaultSound = true,
        SoundSpecifier? announcementSound = null,
        Color? colorOverride = null)
    {
        // broadcast ONLY to the same map
        var activeShips = EntityQuery<ShipTrackerComponent, TransformComponent>()
            .Where(x => x.Item2.MapID == map);

        foreach (var ship in activeShips.ToList())
        {
            // rider im not making obsolete code for the LOVE OF GOD
            _chatSystem.DispatchStationAnnouncement(ship.Item1.Owner, message, sender, playDefaultSound, announcementSound, colorOverride);
        }
    }

    private void OnFTLStartedEvent(EntityUid uid, ShipTrackerComponent component, ref FTLStartedEvent args)
    {
        // alert those who are going onto map
        // BroadcastToStationsOnMap(args.TargetCoordinates.GetMapId(_entityManager), Loc.GetString("ship-ftl-jump-jumped-message"), colorOverride: Color.Gold);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var allShips = EntityQueryEnumerator<ShipTrackerComponent>();

        while (allShips.MoveNext(out var entity, out var shipTrackerComponent))
        {
            if (shipTrackerComponent.Destroyed)
                continue;

            var xform = Transform(entity);
            var activeShuttleComps = EntityQuery<ShuttleConsoleComponent, TransformComponent>()
                .Count(tuple => tuple.Item2.GridUid == entity);

            if (activeShuttleComps > 0)
            {
                // not destroyed, aka piloting is there
                shipTrackerComponent.SecondsWithoutPiloting = 0f;
                continue;
            }

            shipTrackerComponent.SecondsWithoutPiloting += frameTime;
            if (shipTrackerComponent.SecondsWithoutPiloting < shipTrackerComponent.CallDestroyedSeconds)
                continue;

            var ev = new ShipTrackerDestroyed(entity, shipTrackerComponent);
            RaiseLocalEvent(ev);

            shipTrackerComponent.Destroyed = true;

            // BroadcastToStationsOnMap(xform.MapID, Loc.GetString("ship-destroyed-message",
            //     ("ship", MetaData(entity).EntityName)));
        }
    }
}
