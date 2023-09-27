using Robust.Shared.Map;

namespace Content.Server._FTL.ShipWeapons;

/// <summary>
/// This is used for tracking a ship weapon
/// </summary>
[RegisterComponent]
public sealed partial class ShipWeaponComponent : Component
{
    public Angle DesiredAngle = Angle.Zero;
    public EntityCoordinates Target = EntityCoordinates.Invalid;
}
