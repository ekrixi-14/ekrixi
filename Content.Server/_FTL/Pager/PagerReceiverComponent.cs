using Robust.Shared.Audio;

namespace Content.Server._FTL.Pager;

[RegisterComponent]
public sealed partial class PagerReceiverComponent : Component
{
    [DataField]
    public bool Paged;

    [DataField]
    public string PagerMessage = "";

    [DataField]
    public SoundSpecifier PagingSound = new SoundPathSpecifier("/Audio/_FTL/Effects/Pagers/pager_beep.ogg");

    [DataField]
    public SoundSpecifier BeepSound = new SoundPathSpecifier("/Audio/_FTL/Machines/beep.ogg");

    public EntityUid? PlayingStream;
}
