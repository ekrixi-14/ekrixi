using System.Linq;
using Content.Server._FTL.AutomatedShip.Components;
using Content.Server._FTL.ShipTracker.Components;
using Content.Server.NPC.Systems;
using Content.Server.Shuttles.Components;
using Content.Server.Weapons.Ranged.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.Random;

namespace Content.Server._FTL.AutomatedShip.Systems;

/// <summary>
/// This handles AI control
/// </summary>
public sealed partial class AutomatedShipSystem : EntitySystem
{
    [Dependency] private readonly NpcFactionSystem _npcFactionSystem = default!;
    [Dependency] private readonly MetaDataSystem _metaDataSystem = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly GunSystem _gunSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AutomatedShipComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, AutomatedShipComponent component, ComponentStartup args)
    {
        EnsureComp<ActiveAutomatedShipComponent>(uid);
        UpdateName(uid, component);
    }

    private void UpdateName(EntityUid uid, AutomatedShipComponent component)
    {
        var meta = MetaData(uid);
        var tag = "[" + (component.AiState == AutomatedShipComponent.AiStates.Cruising
            ? Loc.GetString("ship-state-tag-neutral")
            : Loc.GetString("ship-state-tag-hostile")) + "] ";

        // has the tag
        // really shitty way of doing this btw
        if (meta.EntityName.StartsWith("["))
        {
            _metaDataSystem.SetEntityName(uid, string.Concat(tag, meta.EntityName.Substring(7, meta.EntityName.Length - 7)));
            return;
        }
        _metaDataSystem.SetEntityName(uid, tag + meta.EntityName);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<AutomatedShipComponent, TransformComponent, ShipTrackerComponent>();
        while (query.MoveNext(out var entity, out var aiComponent, out var xform, out var aiTrackerComponent))
        {
            if (aiTrackerComponent.Destroyed)
                continue;

            // makes sure it's on the same map, not the same grid, and is hostile
            Log.Debug("Retargeting");

            var hostileShipsEnum = EntityQuery<ShipTrackerComponent>().Where(shipTrackerComponent =>
            {
                var owner = shipTrackerComponent.Owner;
                var otherTransform = Transform(owner);

                Log.Debug(
                    $"Same map: {otherTransform.MapID == xform.MapID}, Different grid: {otherTransform.GridUid != xform.GridUid}, Hostile: {_npcFactionSystem.IsFactionHostile(aiTrackerComponent.Faction,
                        shipTrackerComponent.Faction)}");

                return otherTransform.MapID == xform.MapID && otherTransform.GridUid != xform.GridUid &&
                       (_npcFactionSystem.IsFactionHostile(aiTrackerComponent.Faction,
                            shipTrackerComponent.Faction) ||
                        aiComponent.HostileShips.Contains(owner));
            });

            if (!hostileShipsEnum.Any())
                continue;
            var hostileShips = hostileShipsEnum.ToList();
            Log.Debug("Reset retarget");

            var mainShip = _random.Pick(hostileShips).Owner;
            UpdateName(entity, aiComponent);

            // I seperated these into partial systems because I hate large line counts!!!
            Log.Debug("Determining best course");
            switch (aiComponent.AiState)
            {
                case AutomatedShipComponent.AiStates.Cruising:
                {
                    if (hostileShips.Count > 0)
                    {
                        aiComponent.AiState = AutomatedShipComponent.AiStates.Fighting;
                        Log.Debug("Hostile ship inbound!");
                    }
                    break;
                }
                case AutomatedShipComponent.AiStates.Fighting:
                {
                    if (hostileShips.Count <= 0)
                    {
                        aiComponent.AiState = AutomatedShipComponent.AiStates.Cruising;
                        Log.Debug("Lack of a hostile ship.");
                        break;
                    }

                    // var gyroscope = EntityQuery<ThrusterComponent, TransformComponent>().Where(component => component.Item1.Type == ThrusterType.Angular && component.Item2.GridUid == entity );
                    //
                    // if (gyroscope.Any())
                    // {
                    //     var angle = (_entityManager.GetCoordinates(xform.LocalPosition).ToMapPos(_entityManager, _transformSystem) - entityXform.MapPosition.Position).ToWorldAngle();
                    //     _transformSystem.SetWorldRotation(entity, angle);
                    // }

                    PerformCombat(entity, mainShip);
                    break;
                }
                default:
                {
                    Log.Fatal("Non-existent AI state!");
                    break;
                }
            }
        }
    }
}
