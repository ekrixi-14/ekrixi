using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._FTL.ShipWeapons;

public abstract class SharedShipWeaponsSystem : EntitySystem
{

}

[Serializable, NetSerializable]
public enum ShipWeaponTargetingUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public enum ShipWeaponAction : byte
{
    Fire,
    Chamber,
    Eject
}

[Serializable, NetSerializable]
public sealed class RotateWeaponSendMessage : BoundUserInterfaceMessage
{
    public EntityCoordinates Coordinates;

    public RotateWeaponSendMessage(EntityCoordinates coordinates)
    {
        Coordinates = coordinates;
    }
}

[Serializable, NetSerializable]
public sealed class PerformActionWeaponSendMessage : BoundUserInterfaceMessage
{
    public ShipWeaponAction Action;

    public PerformActionWeaponSendMessage(ShipWeaponAction action)
    {
        Action = action;
    }
}
