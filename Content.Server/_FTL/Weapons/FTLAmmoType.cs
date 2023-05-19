using Robust.Shared.Prototypes;

namespace Content.Server._FTL.Weapons;

/// <summary>
/// This is a prototype for tracking FTL ammo
/// </summary>
[Prototype("ftlAmmo")]
public sealed class FTLAmmoType : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;
}
