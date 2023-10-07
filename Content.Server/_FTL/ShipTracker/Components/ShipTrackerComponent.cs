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
    /// The faction of the current ship.
    /// </summary>
    [DataField("faction")]
    [ViewVariables(VVAccess.ReadWrite)]
    public string Faction = "IndependentShip";

    /// <summary>
    /// Is the ship destroyed?
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public bool Destroyed = false;
}
