using System.Linq;
using Content.Server._FTL.AutomatedShip.Components;
using Content.Server._FTL.FTLPoints.Components;
using Content.Server._FTL.FTLPoints.Systems;
using Content.Server._FTL.ShipTracker.Components;
using Content.Server.AlertLevel;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Systems;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Events;
using Content.Server.Station.Systems;
using Content.Shared._FTL.ShipTracker;
using Content.Shared.Pinpointer;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;

namespace Content.Server._FTL.ShipTracker.Systems;

/// <summary>
/// This handles tracking ships, healths and more
/// </summary>
public sealed partial class ShipTrackerSystem : SharedShipTrackerSystem
{
    [Dependency] private readonly AlertLevelSystem _alertLevelSystem = default!;
    [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShipTrackerComponent, FTLCompletedEvent>(OnFTLCompletedEvent);
        SubscribeLocalEvent<ShipTrackerComponent, FTLStartedEvent>(OnFTLStartedEvent);
        SubscribeLocalEvent<ShipTrackerComponent, FTLRequestEvent>(OnFTLRequestEvent);
    }

    private void OnFTLRequestEvent(EntityUid uid, ShipTrackerComponent component, ref FTLRequestEvent args)
    {
        _chatSystem.DispatchGlobalAnnouncement(Loc.GetString("ship-ftl-jump-jumped-message"), colorOverride: Color.Gold);
    }

    private void OnFTLStartedEvent(EntityUid uid, ShipTrackerComponent component, ref FTLStartedEvent args)
    {
        if (args.FromMapUid != null)
            Del(args.FromMapUid.Value);

        _chatSystem.DispatchStationAnnouncement(uid, Loc.GetString("ship-ftl-jump-jumped-message"), colorOverride: Color.Gold);
    }

    private void OnFTLCompletedEvent(EntityUid uid, ShipTrackerComponent component, ref FTLCompletedEvent args)
    {
        var mapId = Transform(args.MapUid).MapID;
        var amount = EntityQuery<AutomatedShipComponent, TransformComponent>().Select(
            tuple => tuple.Item2.MapID == mapId
        ).Count();
        var hostile = EntityQuery<AutomatedShipComponent>().Select(x => x.AiState == AutomatedShipComponent.AiStates.Fighting).Count();

        if (amount > 0)
        {
            _chatSystem.DispatchGlobalAnnouncement(
                Loc.GetString("ship-inbound-message",
                    ("amount", amount),
                    ("hostile", hostile > 0)
                )
            );

            var station = _stationSystem.GetOwningStation(args.Entity);

            if (!station.HasValue)
                return;

            _alertLevelSystem.SetLevel(station.Value, "blue", true, true, false);
        }
        else
        {
            _chatSystem.DispatchGlobalAnnouncement(Loc.GetString("ship-ftl-jump-arrival-message"),
                colorOverride: Color.Gold);
        }
    }

    public bool TryFindRandomTile(EntityUid targetGrid, out Vector2i tile, out EntityCoordinates targetCoords)
    {
        tile = default;

        targetCoords = EntityCoordinates.Invalid;

        if (!TryComp<MapGridComponent>(targetGrid, out var gridComp))
            return false;

        var found = false;
        var (gridPos, _, gridMatrix) = _transformSystem.GetWorldPositionRotationMatrix(targetGrid);
        var gridBounds = gridMatrix.TransformBox(gridComp.LocalAABB);

        for (var i = 0; i < 10; i++)
        {
            var randomX = _random.Next((int) gridBounds.Left, (int) gridBounds.Right);
            var randomY = _random.Next((int) gridBounds.Bottom, (int) gridBounds.Top);

            tile = new Vector2i(randomX - (int) gridPos.X, randomY - (int) gridPos.Y);
            if (_atmosphereSystem.IsTileSpace(targetGrid, Transform(targetGrid).MapUid, tile,
                    mapGridComp: gridComp)
                || _atmosphereSystem.IsTileAirBlocked(targetGrid, tile, mapGridComp: gridComp))
            {
                continue;
            }

            found = true;
            targetCoords = gridComp.GridTileToLocal(tile);
            break;
        }

        return found;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ShuttleConsoleComponent, TransformComponent>();
        var allShips = EntityQueryEnumerator<ShipTrackerComponent>();

        while (allShips.MoveNext(out var entity, out var shipTrackerComponent))
        {
            if (shipTrackerComponent.Destroyed)
                continue;

            var active = false;
            while (query.MoveNext(out _, out _, out var transformComponent))
            {
                active = transformComponent.GridUid == entity;
                if (active)
                    break;
            }

            if (active)
            {
                // not destroyed, aka piloting is there
                shipTrackerComponent.SecondsWithoutPiloting = 0f;
                continue;
            }

            shipTrackerComponent.SecondsWithoutPiloting += frameTime;
            if (shipTrackerComponent.SecondsWithoutPiloting < shipTrackerComponent.CallDestroyedSeconds)
                continue;
            shipTrackerComponent.Destroyed = true;

            _chatSystem.DispatchGlobalAnnouncement(Loc.GetString("ship-destroyed-message",
                ("ship", MetaData(entity).EntityName)));
        }
    }
}
