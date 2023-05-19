using Robust.Shared.Serialization;

namespace Content.Shared._FTL.Weapons;

public abstract class SharedWeaponTargetingSystem : EntitySystem
{

}

[Serializable, NetSerializable]
public enum WeaponTargetingUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class FireWeaponSendMessage : BoundUserInterfaceMessage
{

}

[Serializable, NetSerializable]
public sealed class WeaponTargetingUserInterfaceState : BoundUserInterfaceState
{
    public bool CanFire;

    public WeaponTargetingUserInterfaceState(bool canFire)
    {
        CanFire = canFire;
    }
}
