using System.Numerics;
using Content.Shared._FTL.FtlPoints;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;

namespace Content.Client._FTL.FtlPoints;

public sealed class StarmapControl : Control
{
    private List<Star> _stars = new List<Star>();
    private float _ppd = 15f;

    private Vector2 _mousePos;
    private readonly Font _font;
    private int _stepSize = 1;

    public StarmapControl()
    {
        IoCManager.InjectDependencies(this);

        var cache = IoCManager.Resolve<IResourceCache>();
        _font = new VectorFont(cache.GetResource<FontResource>("/Fonts/IBMPlexMono/IBMPlexMono-Regular.ttf"), 8);
    }

    public void SetStars(List<Star> stars)
    {
        _stars = stars;
    }

    private Vector2 CalculateOffset()
    {
        return Size / 2;
    }

    protected override void MouseWheel(GUIMouseWheelEventArgs args)
    {
        base.MouseWheel(args);


        if (args.Delta.Y > 0)
            _ppd += _stepSize;
        else if (args.Delta.Y < 0)
            _ppd -= _stepSize;
    }

    protected override void MouseMove(GUIMouseMoveEventArgs args)
    {
        base.MouseMove(args);

        _mousePos = args.GlobalPosition;
    }

    protected override void Draw(DrawingHandleScreen handle)
    {
        base.Draw(handle);

        var offset = CalculateOffset();

        foreach (var star in _stars)
        {
            var position = offset + (star.Position * _ppd);
            var globalPosition = GlobalPosition + position;
            var radius = 5f;
            handle.DrawCircle(position, radius, position == Vector2.Zero ? Color.Blue : Color.Red);

            // TODO: show name on hover
            handle.DrawString(_font, position + new Vector2(10, 0), star.Name);
        }
    }
}
