using Content.Server.UserInterface;
using Content.Shared._FTL.ShipWeapons;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Robust.Server.GameObjects;

namespace Content.Server._FTL.ShipWeapons;

public sealed class ShipWeaponsSystem : SharedShipWeaponsSystem
{
    [Dependency] private readonly UserInterfaceSystem _userInterface = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<GunnerConsoleComponent, AfterActivatableUIOpenEvent>(OnToggleInterface);
    }

    private void UpdateUserInterface(EntityUid uid, GunnerConsoleComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        var consoleXform = Transform(uid);
        TryComp<RadarConsoleComponent>(uid, out var radar);
        var range = radar?.MaxRange ?? SharedRadarConsoleSystem.DefaultMaxRange;
        var state = new GunnerConsoleBoundInterfaceState(100, 100, range, consoleXform?.Coordinates, consoleXform?.LocalRotation);
        _userInterface.TrySetUiState(uid, ShipWeaponTargetingUiKey.Key, state);
    }

    private void OnToggleInterface(EntityUid uid, GunnerConsoleComponent component, AfterActivatableUIOpenEvent args)
    {
        UpdateUserInterface(uid, component);
    }
}
