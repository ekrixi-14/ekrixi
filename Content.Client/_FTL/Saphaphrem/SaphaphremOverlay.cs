using Content.Shared._FTL.Saphaphrem;
using Content.Shared.StatusEffect;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._FTL.Saphaphrem;

public sealed class SaphaphremOverlay : Overlay
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntitySystemManager _sysMan = default!;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    public override bool RequestScreenTexture => true;
    private readonly ShaderInstance _rainbowShader;

    public float Intoxication = 0.0f;

    private const float VisualThreshold = 10.0f;
    private const float PowerDivisor = 250.0f;

    public event Action<float>? IntoxicationChanged;

    private float EffectScale => Math.Clamp((Intoxication - VisualThreshold) / PowerDivisor, 0.0f, 1.0f);

    public SaphaphremOverlay()
    {
        IoCManager.InjectDependencies(this);
        _rainbowShader = _prototypeManager.Index<ShaderPrototype>("Saphaphrem").InstanceUnique();
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        var playerEntity = _playerManager.LocalEntity;

        if (playerEntity == null)
            return;

        if (!_entityManager.HasComponent<SaphaphremOverlayComponent>(playerEntity)
            || !_entityManager.TryGetComponent<StatusEffectsComponent>(playerEntity, out var status))
            return;

        var statusSys = _sysMan.GetEntitySystem<StatusEffectsSystem>();
        if (!statusSys.TryGetTime(playerEntity.Value, SaphaphremOverlaySystem.RainbowKey, out var time, status))
            return;

        var timeLeft = ((float) (time.Value.Item2 - time.Value.Item1).TotalSeconds) * 6;
        Intoxication += ((timeLeft - Intoxication) * args.DeltaSeconds / 8f) * 2000;
        IntoxicationChanged!.Invoke(Intoxication);
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        if (!_entityManager.TryGetComponent(_playerManager.LocalEntity, out EyeComponent? eyeComp))
            return false;

        if (args.Viewport.Eye != eyeComp.Eye)
            return false;

        return EffectScale > 0;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null)
            return;

        var handle = args.WorldHandle;
        _rainbowShader.SetParameter("SCREEN_TEXTURE", ScreenTexture);
        _rainbowShader.SetParameter("effectScale", EffectScale);
        handle.UseShader(_rainbowShader);
        handle.DrawRect(args.WorldBounds, Color.White);
        handle.UseShader(null);
    }
}
