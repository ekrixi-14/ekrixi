namespace Content.Server._FTL.HeatSeeking;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class HeatSeekingComponent : Component
{
    [DataField("seekRange")]
    public float DefaultSeekingRange = 100f;

    [DataField]
    public EntityUid? TargetEntity;

    [DataField]
    public float Speed = 10f;
}
