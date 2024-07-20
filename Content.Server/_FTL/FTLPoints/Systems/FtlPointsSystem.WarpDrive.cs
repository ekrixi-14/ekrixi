using System.Linq;
using Content.Server._FTL.FTLPoints.Components;
using Content.Server.Shuttles.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Robust.Shared.Map;

namespace Content.Server._FTL.FTLPoints.Systems;

public sealed partial class FtlPointsSystem
{
    /// <summary>
    /// Will attempt to find ONE drive on the grid. If two or more are found, then it will return false.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="drive"></param>
    /// <returns></returns>
    public bool TryFindDriveOnGrid(EntityUid grid, out WarpDriveComponent? drive)
    {
        var drives = EntityQuery<WarpDriveComponent>().ToList().FindAll(x => Transform(x.Owner).GridUid == grid);
        if (drives.Count == 1)
        {
            drive = drives[0];
            return true;
        }

        drive = null;
        return false;
    }

    private void OnDriveInteractHand(EntityUid uid, WarpDriveComponent component, InteractHandEvent args)
    {
        var xform = Transform(uid);
        if (xform.GridUid == null)
            return; // we need a grid uid

        var grid = xform.GridUid.Value;
        if (!TryComp<WarpingShipComponent>(grid, out var warpingShipComponent))
            return;

        component.Charging = !component.Charging;
        _popupSystem.PopupEntity(Loc.GetString(component.Charging ? "popup-drive-charging" : "popup-drive-not-charging"), uid);
    }

    private void DriveUpdate(float frameTime)
    {
        var query = EntityQueryEnumerator<WarpDriveComponent, TransformComponent>();
        while (query.MoveNext(out var entity, out var component, out var xform))
        {
            if (!xform.GridUid.HasValue)
                return;

            var grid = xform.GridUid.Value;
            var warpingShipComponent = EnsureComp<WarpingShipComponent>(grid);

            if (!TryFindDriveOnGrid(grid, out _))
                continue;

            if (!TryComp<ShuttleComponent>(grid, out var shuttleComponent))
                return;

            if (component.Charging && component.Charge < component.ChargeNeeded)
                component.Charge += frameTime;

            if (component.Charge < component.ChargeNeeded)
                continue;

            if (!warpingShipComponent.TargetMap.HasValue)
                continue;

            if (!_mapSystem.TryGetMap(warpingShipComponent.TargetMap.Value, out var tmEntity))
                continue;

            var coordinates = new EntityCoordinates(tmEntity.Value,
                GenerateVectorWithRandomRadius(50, 150),
                GenerateVectorWithRandomRadius(50, 150));
            _shuttleSystem.FTLToCoordinates(grid, shuttleComponent, coordinates, Angle.Zero);
            warpingShipComponent.TargetMap = null;
            component.Charge = 0;
            component.Charging = false;
        }
    }

    private void OnDriveExamineEvent(EntityUid uid, WarpDriveComponent component, ExaminedEvent args)
    {
        var xform = Transform(uid);
        if (!xform.GridUid.HasValue)
            return;

        var grid = xform.GridUid.Value;
        var warpingShipComponent = EnsureComp<WarpingShipComponent>(grid);

        if (!TryFindDriveOnGrid(grid, out _))
        {
            args.PushMarkup(Loc.GetString("drive-examined-multiple-drives"));
            return;
        }

        if (component.Charge >= component.ChargeNeeded)
        {
            args.PushMarkup(Loc.GetString("drive-examined-ready"));
            return;
        }

        args.PushMarkup(Loc.GetString("drive-examined", ("charging", component.Charging), ("charge",
            $"{(component.Charge / component.ChargeNeeded * 100):F}"), ("destination", warpingShipComponent.TargetMap != null)));
    }
}
