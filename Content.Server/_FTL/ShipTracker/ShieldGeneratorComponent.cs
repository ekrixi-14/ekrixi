using Robust.Shared.Audio;

namespace Content.Server._FTL.ShipTracker;

/// <summary>
/// By having this on a grid, they get +1 shield capacity
/// </summary>
[RegisterComponent]
public sealed partial class ShieldGeneratorComponent : Component
{
    /// <summary>
    /// Sound that is played when the shield is damaged
    /// </summary>
    [DataField("damageSound")] public SoundSpecifier DamageSound = new SoundPathSpecifier("/Audio/Effects/radpulse1.ogg");

    /// <summary>
    /// Sound that is played when the shield is recharged
    /// </summary>
    [DataField("rechargeSound")] public SoundSpecifier RechargeSound = new SoundPathSpecifier("/Audio/Effects/sparks3.ogg");

    /// <summary>
    /// Whether this generator counts to the shield count or not
    /// </summary>
    public bool Enabled = true;
}
