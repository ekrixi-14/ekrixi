using Robust.Shared.Containers;

namespace Content.Shared._FTL.Wounds;

[RegisterComponent]
public sealed partial class WoundsHolderComponent : Component
{
    [DataField, ViewVariables] public Container Wounds = default!;
}
