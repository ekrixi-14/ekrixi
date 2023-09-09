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
