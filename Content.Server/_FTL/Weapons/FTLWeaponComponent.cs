using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._FTL.Weapons;

/// <summary>
/// This is used for tracking weapons.
/// </summary>
[RegisterComponent]
public sealed class FTLWeaponComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)] public bool CanBeUsed;

    [DataField("prototype", customTypeSerializer: typeof(PrototypeIdSerializer<FTLAmmoType>))]
    public string Prototype { get; set; } = "";
}
