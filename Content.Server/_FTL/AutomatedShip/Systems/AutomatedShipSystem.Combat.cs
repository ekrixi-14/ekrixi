using Content.Server._FTL.AutomatedShip.Components;
using Content.Server._FTL.ShipTracker.Components;
using Content.Server._FTL.ShipWeapons;
using Content.Shared.DeviceLinking;
using Content.Shared.Physics;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Physics;
using Robust.Shared.Random;

namespace Content.Server._FTL.AutomatedShip.Systems;

public sealed partial class AutomatedShipSystem
{
    private List<EntityUid> GetWeaponGroupsOnGrid(EntityUid gridUid)
    {
        var weapons = new List<EntityUid>();
        var query = EntityQueryEnumerator<GunnerConsoleComponent, TransformComponent>();
        while (query.MoveNext(out var entity, out _, out var transformComponent))
        {
            if (transformComponent.GridUid == gridUid)
                weapons.Add(entity);
        }

        return weapons;
    }

    /// <summary>
    /// Gets a list of priority entities on the grid.
    /// </summary>
    /// <param name="grid">Grid UID</param>
    /// <returns>A list of priority entities</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public List<EntityUid> GetPriorityEntities(EntityUid grid)
    {
        var query = EntityQueryEnumerator<PriorityEntityComponent, TransformComponent>();
        var list = new List<EntityUid>();

        while (query.MoveNext(out var entity, out _, out var transformComponent))
        {
            if (transformComponent.GridUid != grid)
                continue;
            list.Add(entity);
        }

        list.Sort((a, b) =>
        {
            if (!TryComp<PriorityEntityComponent>(a, out var aP))
                return 1;
            if (!TryComp<PriorityEntityComponent>(b, out var bP))
                return -1;
            return aP.Priority.CompareTo(bP.Priority);
        });

        return list;
    }

    private CombatResult PerformCombat(
        EntityUid entity,
        AutomatedShipComponent aiComponent,
        ShipTrackerComponent shipTrackerComponent,
        EntityUid targetShip
        )
    {
        Log.Debug("Les go!!1!!11");
        // realistically this should factor in distance but ¯\_(ツ)_/¯
        var weaponGroup = _random.Pick(GetWeaponGroupsOnGrid(entity));
        // get gun, aim, and shoot at place
        if (!TryComp<DeviceLinkSourceComponent>(weaponGroup, out var sourceComponent))
            return CombatResult.ERROR;
        Log.Debug("got device!!");

        // get a list of prio entities and select one
        var prioEnts = GetPriorityEntities(targetShip);
        if (prioEnts.Count <= 0)
            return CombatResult.NOPRIOENT;
        Log.Debug("there's more prioents!!!");
        var prioEnt = _random.Pick(prioEnts);
        var prioTransform = Transform(prioEnt);

        // yeah we loop through everything
        foreach (var (_, outputs) in sourceComponent.Outputs)
        {
            foreach (var weaponEntity in outputs)
            {
                // we cant really rotate without a location so...
                if (!TryComp<ShipWeaponComponent>(weaponEntity, out var shipWeaponComponent))
                    continue;
                if (!TryComp<GunComponent>(weaponEntity, out var gunComponent))
                    continue;
                var gunTransform = Transform(weaponEntity);

                var angle = (prioTransform.Coordinates.ToMapPos(_entityManager, _transformSystem) - gunTransform.MapPosition.Position).ToWorldAngle();
                shipWeaponComponent.DesiredAngle = angle;
                shipWeaponComponent.Target = prioTransform.Coordinates;

                // don't shoot if it will hit ourselves
                var ray = new CollisionRay(gunTransform.MapPosition.Position, gunTransform.LocalRotation.ToWorldVec(), (int) (CollisionGroup.Impassable | CollisionGroup.BulletImpassable));
                var results = _physics.IntersectRay(gunTransform.MapID, ray, 1000f, weaponEntity);
                var aimHitFriendly = false;

                foreach (var result in results)
                {
                    if (!result.HitEntity.IsValid())
                        continue;
                    var hitEntityXform = Transform(result.HitEntity);
                    if (hitEntityXform.GridUid != gunTransform.GridUid)
                        continue;
                    aimHitFriendly = true;
                }

                if (aimHitFriendly)
                    continue;

                // fire!
                if (!_gunSystem.CanShoot(gunComponent))
                    continue;

                _gunSystem.AttemptShoot(weaponEntity, weaponEntity, gunComponent, shipWeaponComponent.Target);
            }
        }

        return CombatResult.OK;
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

    enum CombatResult
    {
        OK,
        NOPRIOENT,
        ERROR
    }
}
