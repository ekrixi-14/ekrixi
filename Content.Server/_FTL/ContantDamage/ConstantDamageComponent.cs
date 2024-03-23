using Content.Shared.Damage;

namespace Content.Server._FTL.ContantDamage;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class ConstantDamageComponent : Component
{
    [DataField] public float CheckFrequency;
    [DataField] public float Probability;
    [DataField] public DamageSpecifier Damage;

    public float TimeSinceLastCheck;
}
