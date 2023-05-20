namespace Content.Server._FTL.Weapons;

/// <summary>
/// This is used for tracking a weapon pad.
/// </summary>
[RegisterComponent]
public sealed class WeaponTargetingComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)] public bool CanFire = true;
    [DataField("cooldownTime"), ViewVariables(VVAccess.ReadWrite)]
    public float CooldownTime = 5f;
}

/// <summary>
/// Added to an entity using station map so when its parent changes we reset it.
/// </summary>
[RegisterComponent]
public sealed class WeaponTargetingUserComponent : Component
{
    [DataField("mapUid")] public EntityUid Map;
}
