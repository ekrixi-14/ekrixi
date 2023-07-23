using System.Linq;
using Content.Server._FTL.AutomatedShip.Components;
using Content.Server._FTL.ShipTracker;
using Content.Server._FTL.Weapons;
using Robust.Shared.Random;

namespace Content.Server._FTL.AutomatedShip.Systems;

public sealed partial class AutomatedShipSystem
{
    private List<EntityUid> GetWeaponsOnGrid(EntityUid gridUid)
    {
        var weapons = new List<EntityUid>();
        var query = EntityQueryEnumerator<FTLWeaponComponent, TransformComponent>();
        while (query.MoveNext(out var entity, out var weapon, out var xform))
        {
            if (xform.GridUid == gridUid && weapon.CanBeUsed)
            {
                weapons.Add(entity);
            }
        }

        return weapons;
    }

    private void PerformCombat(
        EntityUid entity,
        ActiveAutomatedShipComponent activeComponent,
        AutomatedShipComponent aiComponent,
        TransformComponent transformComponent,
        ShipTrackerComponent aiTrackerComponent
        )
    {
        if (activeComponent.TimeSinceLastAttack >= aiComponent.AttackRepetition)
        {
            var transform = transformComponent;
            // makes sure it's on the same map, not the same grid, and is hostile
            var otherShips = EntityQuery<ShipTrackerComponent>().Where(shipTrackerComponent => Transform(shipTrackerComponent.Owner).MapID == transform.MapID && Transform(shipTrackerComponent.Owner).GridUid != transform.GridUid && _npcFactionSystem.IsFactionHostile(aiTrackerComponent.Faction, shipTrackerComponent.Faction)).ToList();

            if (otherShips.Count <= 0)
                return;

            var mainShip = _random.Pick(otherShips).Owner;

            var weapons = GetWeaponsOnGrid(entity);
            var weapon = _random.Pick(weapons);

            if (TryComp<FTLWeaponComponent>(weapon, out var weaponComponent) && TryFindRandomTile(mainShip, out _, out var coordinates))
            {
                activeComponent.TimeSinceLastAttack = 0;
                Log.Debug(coordinates.ToString());
                _weaponTargetingSystem.TryFireWeapon(weapon, weaponComponent, mainShip, coordinates, null);
            }
        }
    }
}
