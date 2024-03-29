using System.Linq;
using Content.Server._FTL.FTLPoints.Components;
using Content.Server.Shuttles.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

            if (component is { Charging: true, Charge: < 1 })
                component.Charge += frameTime / 30;

            if (!(component.Charge >= 1))
                continue;

            if (!warpingShipComponent.TargetMap.HasValue)
                continue;

            _shuttleSystem.FTLTravel(grid, shuttleComponent, _mapManager.GetMapEntityId(warpingShipComponent.TargetMap.Value));
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

        if (component.Charge >= 1)
        {
            args.PushMarkup(Loc.GetString("drive-examined-ready"));
            return;
        }

        args.PushMarkup(Loc.GetString("drive-examined", ("charging", component.Charging), ("charge",
            $"{(component.Charge * 100):F}"), ("destination", warpingShipComponent.TargetMap != null)));
    }
}
