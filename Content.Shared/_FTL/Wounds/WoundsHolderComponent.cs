using Robust.Shared.Containers;
using Robust.Shared.GameStates;

namespace Content.Shared._FTL.Wounds;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class WoundsHolderComponent : Component
{
    [ViewVariables] public Container Wounds = default!;
    [ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public int CurrentWoundTreating = 0;
}
