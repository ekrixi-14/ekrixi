using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Client._FTL.Hologram;

public sealed class HologramVisualizerSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private static readonly string ShaderName = "Hologram";
    private ShaderInstance _shader = default!;

    public override void Initialize()
    {
        base.Initialize();

        _shader = _protoMan.Index<ShaderPrototype>(ShaderName).InstanceUnique();
        SubscribeLocalEvent<HologramVisualsComponent, ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(EntityUid uid, HologramVisualsComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite))
            return;

        sprite.PostShader = _shader;
    }
}
