using Content.Server._FTL.AutomatedShip.Components;
using Content.Server._FTL.ShipTracker;
using Robust.Shared.Random;

namespace Content.Server._FTL.AutomatedShip.Systems;

public sealed partial class AutomatedShipSystem
{
    // private List<EntityUid> GetWeaponsOnGrid(EntityUid gridUid)
    // {
    //     var weapons = new List<EntityUid>();
    //     var query = EntityQueryEnumerator<FTLWeaponComponent, TransformComponent>();
    //     while (query.MoveNext(out var entity, out var weapon, out var xform))
    //     {
    //         if (xform.GridUid == gridUid && weapon.CanBeUsed)
    //         {
    //             weapons.Add(entity);
    //         }
    //     }
    //
    //     return weapons;
    // }

    private void PerformCombat(
        EntityUid entity,
        ActiveAutomatedShipComponent activeComponent,
        AutomatedShipComponent aiComponent,
        TransformComponent transformComponent,
        ShipTrackerComponent aiTrackerComponent,
        EntityUid mainShip
        )
    {
        if (activeComponent.TimeSinceLastAttack < aiComponent.AttackRepetition)
            return;

        // var weapon = _random.Pick(GetWeaponsOnGrid(entity));
        // properly attack

       activeComponent.TimeSinceLastAttack = 0;
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
