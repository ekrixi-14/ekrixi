namespace Content.Server._FTL.HeatSeeking;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class HeatSeekingComponent : Component
{
    /// <summary>
    /// How far does this fire a raycast onto?
    /// </summary>
    [DataField("seekRange")]
    public float DefaultSeekingRange = 100f;

    /// <summary>
    /// Should this lock onto ONE entity only?
    /// </summary>
    [DataField]
    public bool LockedIn;

    [DataField]
    public Angle WeaponArc = Angle.FromDegrees(360);

    /// <summary>
    /// If null it will instantly turn.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public Angle? RotationSpeed;

    /// <summary>
    /// What is this entity targeting?
    /// </summary>
    [DataField]
    public EntityUid? TargetEntity;

    /// <summary>
    /// How fast does the missile accelerate?
    /// </summary>
    [DataField]
    public float Acceleration = 10f;
}
