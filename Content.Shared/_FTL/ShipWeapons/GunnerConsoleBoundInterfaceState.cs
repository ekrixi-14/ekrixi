using Content.Shared.Shuttles.BUIStates;
using Robust.Shared.Serialization;

namespace Content.Shared._FTL.ShipWeapons;

[Serializable, NetSerializable]
public sealed class GunnerConsoleBoundInterfaceState : BoundUserInterfaceState
{
    public readonly int CurrentAmmo;
    public readonly int MaxAmmo;
    public NavInterfaceState State;

    public GunnerConsoleBoundInterfaceState(
        int currentAmmo,
        int maxAmmo,
        NavInterfaceState state)
    {
        CurrentAmmo = currentAmmo;
        MaxAmmo = maxAmmo;
        State = state;
    }
}
