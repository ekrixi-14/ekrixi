using Content.Shared.Shuttles.BUIStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._FTL.ShipWeapons;

[Serializable, NetSerializable]
public sealed class GunnerConsoleBoundInterfaceState : RadarConsoleBoundInterfaceState
{
    public readonly int CurrentAmmo;
    public readonly int MaxAmmo;
    public readonly List<DockingInterfaceState> Weapons;

    public GunnerConsoleBoundInterfaceState(
        int currentAmmo,
        int maxAmmo,
        float maxRange,
        List<DockingInterfaceState> weapons,
        EntityCoordinates? coordinates,
        Angle? angle) : base(maxRange, coordinates, angle, new List<DockingInterfaceState>())
    {
        CurrentAmmo = currentAmmo;
        MaxAmmo = maxAmmo;
        Weapons = weapons;
    }
}
