namespace Content.Server._FTL.ShipRename;

/// <summary>
/// This is used for tracking renaming ships
/// </summary>
[RegisterComponent]
public sealed partial class ShipRenameComponent : Component
{
    public EntityUid? GridId { set; get; }
}
