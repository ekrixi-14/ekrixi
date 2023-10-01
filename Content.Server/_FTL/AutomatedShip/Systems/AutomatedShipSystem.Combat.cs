using Content.Server._FTL.AutomatedShip.Components;
using Content.Server._FTL.ShipTracker;
using Content.Server._FTL.ShipTracker.Components;
using Content.Server._FTL.ShipWeapons;
using Robust.Shared.Random;

namespace Content.Server._FTL.AutomatedShip.Systems;

public sealed partial class AutomatedShipSystem
{
    private List<EntityUid> GetWeaponGroupsOnGrid(EntityUid gridUid)
    {
        var weapons = new List<EntityUid>();
        var query = EntityQueryEnumerator<GunnerConsoleComponent>();
        while (query.MoveNext(out var entity, out var weapon))
        {
            weapons.Add(entity);
        }

        return weapons;
    }

    private void PerformCombat(
        EntityUid entity,
        ActiveAutomatedShipComponent activeComponent,
        AutomatedShipComponent aiComponent,
        TransformComponent transformComponent,
        ShipTrackerComponent aiTrackerComponent,
        EntityUid mainShip
        )
    {

        // realistically this should factor in distance but ¯\_(ツ)_/¯
        var weaponGroup = _random.Pick(GetWeaponGroupsOnGrid(entity));
        // get gun, aim, and shoot at place
    }

    // private void OnShipDamaged(EntityUid uid, AutomatedShipComponent component, ref ShipDamagedEvent args)
    // {
    //     if (component.AiState == AutomatedShipComponent.AiStates.Fighting || component.HostileShips.Contains(args.Source))
    //         return;
    //     // we were just minding our own business and we were shot at! prepare for combat!
    //     Log.Debug("We've been shot at! Fight back!");
    //     component.AiState = AutomatedShipComponent.AiStates.Fighting;
    //     component.HostileShips.Add(args.Source);
    // }
}
