using System.Numerics;
using Content.Server._FTL.FTLPoints.Systems;

namespace Content.Server._FTL.FTLPoints.Components;

/// <summary>
/// This is used for tracking FTL points and their positioning.
/// </summary>
[RegisterComponent, Access(typeof(FtlPointsSystem), Other = AccessPermissions.Read)]
public sealed partial class StarMapComponent : Component
{
    public Dictionary<Vector2, EntityUid> StarMap = new ();
}
