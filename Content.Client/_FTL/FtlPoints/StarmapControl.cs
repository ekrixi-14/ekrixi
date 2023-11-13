using System.Numerics;
using Content.Shared._FTL.FtlPoints;
using Content.Shared.Input;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Input;
using YamlDotNet.Core.Tokens;

namespace Content.Client._FTL.FtlPoints;

public sealed class StarmapControl : Control
{
    [Dependency] private readonly IInputManager _inputManager = default!;

    public float Range = 1f;

    private List<Star> _stars = new List<Star>();
    private const float Ppd = 15f;

    private readonly Font _font;
    private bool _mouseDown = false;

    public event Action<Star>? OnStarSelect;

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

    private Vector2 GetPositionOfStar(Vector2 position)
    {
        return CalculateOffset() + (position * Ppd);
    }

    protected override void Draw(DrawingHandleScreen handle)
    {
        base.Draw(handle);
        handle.DrawRect(new UIBox2(Vector2.Zero, Size), Color.Black);

        // Draw lines in a grid
        var lines = 10;

        for (var i = 0; i < lines; i++)
        {
            var xStep = Size.X / lines;
            var yStep = Size.X / lines;
            handle.DrawLine(new Vector2(i * xStep, 0), new Vector2(i * xStep, Size.Y), Color.DarkSlateGray);
            handle.DrawLine(new Vector2(0, i * yStep), new Vector2(Size.X, i * yStep), Color.DarkSlateGray);
        }

        // Draw warp range
        handle.DrawCircle(GetPositionOfStar(Vector2.Zero), Range * Ppd, Color.White, false);
        handle.DrawCircle(GetPositionOfStar(Vector2.Zero), Range * Ppd, new Color(47, 79, 79, 127));

        // Draw sensor range
        handle.DrawCircle(GetPositionOfStar(Vector2.Zero), (int) (Range * 1.5) * Ppd, Color.Blue, false);

        foreach (var star in _stars)
        {
            var uiPosition = GetPositionOfStar(star.Position);
            var globalPosition = GlobalPosition + uiPosition;
            var radius = 5f;

            // check if distance is smaller than radius of circle then BOOm
            var hovered = Vector2.Distance(GetMouseCoordinates(), globalPosition) <= radius * 1.5;

            var color = Color.White;
            var name = star.Name;

            // out of warp range
            if (Vector2.Distance(Vector2.Zero, star.Position) >= Range)
                color = Color.Red;

            // out of scanning range
            if (Vector2.Distance(Vector2.Zero, star.Position) >= Range * 1.5)
            {
                color = Color.DarkRed;
                name = Loc.GetString("ship-ftl-tag-oor");
            }

            if (star.Position == Vector2.Zero)
                color = Color.Blue;

            // before circle rendering so that we can change whats rendered
            if (hovered)
            {
                radius = 10f;
            }
            handle.DrawCircle(uiPosition, radius, color);

            // after circle rendering incase we wish to show text/etc
            if (hovered)
            {
                handle.DrawString(_font, uiPosition + new Vector2(10, 0), name);
            }

            // on click
            if (!hovered || !_inputManager.IsKeyDown(Keyboard.Key.MouseLeft))
                continue;

            if (Vector2.Distance(Vector2.Zero, star.Position) >= Range)
                continue; // out of warp range

            OnStarSelect?.Invoke(star);
        }
    }
}
