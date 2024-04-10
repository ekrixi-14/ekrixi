using Robust.Shared.Serialization;

namespace Content.Shared._FTL.Pager;


[Serializable, NetSerializable]
public enum PagerVisualLayers : byte
{
    Base,
    Receiving,
    Paging
}
