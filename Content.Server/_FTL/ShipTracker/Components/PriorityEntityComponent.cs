namespace Content.Server._FTL.ShipTracker.Components;

/// <summary>
/// Marks the entity as a priority object which the AI will prioritize regarding a number of factors (distance, armor, etc)
/// </summary>
[RegisterComponent]
public sealed partial class PriorityEntityComponent : Component
{
    /// <summary>
    /// The priority of this object.
    /// </summary>
    [DataField("priority")] public int Priority = 1;
}
