using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;

namespace Content.Client._FTL.FilmGrain;

public sealed class FilmGrainOverlay : Overlay
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public override bool RequestScreenTexture => true;
    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    private readonly ShaderInstance _shader;

    public FilmGrainOverlay()
    {
        IoCManager.InjectDependencies(this);
        // Load shaders from prototypes
        _shader = _prototypeManager.Index<ShaderPrototype>("FilmGrain").InstanceUnique();
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null)
            return;

        _shader?.SetParameter("SCREEN_TEXTURE", ScreenTexture);

        var handle = args.WorldHandle;
        var viewport = args.WorldBounds;
        // draw the greyscale shader
        handle.UseShader(_shader);
        handle.DrawRect(viewport, Color.White);
    }
}
