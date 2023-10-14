using System.Numerics;
using Content.Shared._FTL.FtlPoints;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;

namespace Content.Client._FTL.FtlPoints;

public sealed class StarmapControl : Control
{
    [Dependency] private readonly IInputManager _inputManager = default!;

    public float Range = 1f;

    private List<Star> _stars = new List<Star>();
    private float _ppd = 15f;

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

    private Vector2 GetMouseCoordinates()
    {
        return _inputManager.MouseScreenPosition.Position;
    }

    protected override void MouseWheel(GUIMouseWheelEventArgs args)
    {
        base.MouseWheel(args);

        switch (args.Delta.Y)
        {
            case > 0:
                _ppd += _stepSize;
                break;
            case < 0:
                _ppd -= _stepSize;
                break;
        }
    }

    private Vector2 GetPositionOfStar(Vector2 position)
    {
        return CalculateOffset() + (position * _ppd);
    }

    protected override void Draw(DrawingHandleScreen handle)
    {
        base.Draw(handle);
        handle.DrawRect(new UIBox2(Vector2.Zero, Size), Color.Black);

        foreach (var star in _stars)
        {
            var position = GetPositionOfStar(star.Position);
            var globalPosition = GlobalPosition + position;
            var radius = 5f;

            // check if distance is smaller than radius of circle then BOOm
            var hovered = Vector2.Distance(GetMouseCoordinates(), globalPosition) <= radius * 1.5;

            if (hovered)
            {
                handle.DrawString(_font, position + new Vector2(10, 0), star.Name);
                radius = 10f;
            }

            var color = Color.White;

            if (Vector2.Distance(Vector2.Zero, position) >= Range)
                color = Color.Red;

            if (star.Position == Vector2.Zero)
                color = Color.Blue;

            handle.DrawCircle(position, radius, color);
        }

        handle.DrawCircle(GetPositionOfStar(Vector2.Zero), Range * _ppd, Color.White, false);
    }
}
