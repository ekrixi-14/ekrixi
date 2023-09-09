using Content.Client.Shuttles.UI;
using Content.Shared._FTL.ShipWeapons;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Events;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client._FTL.ShipWeapons;

[UsedImplicitly]
public sealed class GunnerConsoleBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private GunnerConsoleWindow? _window;

    public GunnerConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();
        _window = new GunnerConsoleWindow();
        _window.OpenCentered();
        _window.OnClose += OnClose;
    }

    private void OnClose()
    {
        Close();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _window?.Dispose();
        }
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        if (state is not GunnerConsoleBoundInterfaceState cState)
            return;

        _window?.SetMatrix(cState.Coordinates, cState.Angle);
        _window?.UpdateState(cState);
    }
}
