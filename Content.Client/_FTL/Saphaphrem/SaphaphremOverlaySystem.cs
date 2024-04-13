using Content.Shared._FTL.Saphaphrem;
using Content.Shared.Drugs;
using Robust.Client.Audio;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Player;

namespace Content.Client._FTL.Saphaphrem;

/// <summary>
///     System to handle drug related overlays.
/// </summary>
/// <remarks>
/// Did I copy and paste this from Content.Client.Drugs? Yes. Shamelessly too. I'll write a proper drug overlay API.
/// </remarks>
public sealed class SaphaphremOverlaySystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly AudioSystem _audioSystem = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    private SaphaphremOverlay _overlay = default!;

    public static string RainbowKey = "SaphaphremTrip";
    private EntityUid? _attachedPlayer;
    private (EntityUid Entity, AudioComponent Component)? _audio;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SaphaphremOverlayComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<SaphaphremOverlayComponent, ComponentShutdown>(OnShutdown);

        SubscribeLocalEvent<SaphaphremOverlayComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<SaphaphremOverlayComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);

        _overlay = new();
        _overlay.IntoxicationChanged += IntoxicationChanged;
    }

    private void IntoxicationChanged(float intoxication)
    {
        Logger.Debug(intoxication.ToString());
        Logger.Debug(_attachedPlayer.HasValue.ToString());

        if (intoxication >= 1 && _attachedPlayer.HasValue)
        {
            if (_audio is { Component.Playing: true })
            {
                Log.Debug("Already playin.......");
                return; // no need to continue playing an actively playing track
            }

            var x = _audioSystem.PlayGlobal("/Audio/_FTL/Misc/AmenBreak/ftl_break.ogg", _attachedPlayer.Value, new AudioParams()
            {
                Loop = true
            });
            if (!x.HasValue)
                 return;

            _audio = x;
        }
        else
        {
            if (!_audio.HasValue)
                return;
            _audioSystem.Stop(_audio.Value.Entity);
            _audio = null;
        }
    }

    private void OnPlayerAttached(EntityUid uid, SaphaphremOverlayComponent component, LocalPlayerAttachedEvent args)
    {
        _overlayMan.AddOverlay(_overlay);
        _attachedPlayer = uid;
    }

    private void OnPlayerDetached(EntityUid uid, SaphaphremOverlayComponent component, LocalPlayerDetachedEvent args)
    {
        _overlay.Intoxication = 0;
        _overlayMan.RemoveOverlay(_overlay);
    }

    private void OnInit(EntityUid uid, SaphaphremOverlayComponent component, ComponentInit args)
    {
        if (_player.LocalEntity == uid)
            _overlayMan.AddOverlay(_overlay);
        _attachedPlayer = uid;
    }

    private void OnShutdown(EntityUid uid, SaphaphremOverlayComponent component, ComponentShutdown args)
    {
        if (_player.LocalEntity != uid)
            return;
        _overlay.Intoxication = 0;
        _overlayMan.RemoveOverlay(_overlay);
    }
}
