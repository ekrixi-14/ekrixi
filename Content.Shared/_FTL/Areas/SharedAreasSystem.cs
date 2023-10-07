using Robust.Shared.Serialization;

namespace Content.Shared._FTL.Areas;

[Serializable, NetSerializable]
public struct Area
{
    public NetEntity Entity;
    public string Name;
    public bool Enabled;
}

public abstract class SharedAreasSystem : EntitySystem
{

}
