using Robust.Client.Animations;

namespace Content.Client._FTL.Doors;

[RegisterComponent]
public sealed partial class BayAirlockVisualsComponent : Component
{

    [DataField("doorColor")]
    public Color? DoorColor;

    [DataField("stripeColor")]
    public Color? StripeColor;

    [DataField("delay")]
    public float Delay = 0.6f;

    public Animation CloseAnimation = default!;
    public Animation OpenAnimation = default!;
    public Animation DenyAnimation = default!;
    public Animation EmagAnimation = default!;
}

public enum BayDoorAnimatedLayers
{
    Base,
    Color,
    ColorFill,
    Stripe,
    StripeFill,
}

public enum BayDoorAncillaryLayers
{
    BoltLights,
    GreenLights,
    DenyLights,
    Emag,
    Welded
}
