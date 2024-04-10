using Robust.Shared.Audio;

namespace Content.Server._FTL.Pager;

[RegisterComponent]
public sealed partial class PagerActionsComponent : Component
{
    [DataField]
    public Dictionary<string, string> Aliases = new ();
}
