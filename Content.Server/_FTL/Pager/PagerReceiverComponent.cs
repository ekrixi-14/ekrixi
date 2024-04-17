using Robust.Shared.Audio;

namespace Content.Server._FTL.Pager;

[RegisterComponent]
public sealed partial class PagerReceiverComponent : Component
{
    [DataField, ViewVariables]
    public bool Paged;

    [DataField, ViewVariables]
    public string PagerMessage = "";

    [DataField, ViewVariables]
    public SoundSpecifier PagingSound = new SoundPathSpecifier("/Audio/_FTL/Effects/Pagers/pager_beep.ogg");

    [DataField, ViewVariables]
    public SoundSpecifier BeepSound = new SoundPathSpecifier("/Audio/_FTL/Machines/beep.ogg");

    [DataField,
    public EntityUid? PlayingStream;
}
