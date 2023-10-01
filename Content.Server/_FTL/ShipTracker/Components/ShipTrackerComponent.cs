using Content.Server._FTL.ShipTracker.Systems;

namespace Content.Server._FTL.ShipTracker.Components;

/// <summary>
/// This is used for tracking the damage on ships
/// </summary>
[RegisterComponent]
[Access(typeof(ShipTrackerSystem))]
public sealed partial class ShipTrackerComponent : Component
{
    /// <summary>
    /// The maximum capacity of the shields
    /// </summary>
    [DataField("faction")]
    [ViewVariables(VVAccess.ReadWrite)]
    public string Faction = "IndependentShip";
}
