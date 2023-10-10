using System.Numerics;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._FTL.FtlPoints;

public abstract class SharedFtlPointsSystem : EntitySystem
{

}

[NetSerializable, Serializable]
public enum StarmapConsoleUiKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class StarmapConsoleBoundUserInterfaceState : BoundUserInterfaceState
{
    public List<Star> Stars;

    public StarmapConsoleBoundUserInterfaceState(List<Star> stars)
    {
        Stars = stars;
    }
}

[Serializable, NetSerializable]
public struct Star
{
    public Vector2 Position;
    public string Name;
    public MapId Map;

    public Star(Vector2 position, MapId map, string name)
    {
        Position = position;
        Name = name;
        Map = map;
    }
}
