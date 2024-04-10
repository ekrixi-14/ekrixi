using Robust.Shared.Audio;

namespace Content.Server._FTL.Pager;

[RegisterComponent]
public sealed partial class PagerReceiverComponent : Component
{
    [DataField]
    public bool Paged;

    [DataField]
    public SoundSpecifier PagingSound = new SoundPathSpecifier("/Audio/_FTL/Effects/Pagers/pager_beep.ogg");

    [DataField]
    public SoundSpecifier BeepSound = new SoundPathSpecifier("/Audio/Effects/beep1.ogg");

    public EntityUid? PlayingStream;
}
