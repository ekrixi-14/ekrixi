using Content.Shared._FTL.FtlPoints;
using Robust.Shared.Prototypes;
using JetBrains.Annotations;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._FTL.FtlPoints;

[UsedImplicitly]
public sealed class StarmapConsoleBoundUserInterface : BoundUserInterface
{
    private StarmapConsole? _window;

    public StarmapConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey) {}

    protected override void Open()
    {
        base.Open();
        var collection = IoCManager.Instance;

        if (collection == null)
            return;

        _window = new StarmapConsole(this, IoCManager.Resolve<IPrototypeManager>());
        _window.OnClose += Close;

        _window.OpenCentered();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _window?.Dispose();
        }
    }

    public void StarWarpButtonOnOnPressed(Star star)
    {
        var message = new WarpToStarMessage(star);
        SendMessage(message);
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        var castState = (StarmapConsoleBoundUserInterfaceState) state;
        _window?.UpdateState(castState);
    }
}
