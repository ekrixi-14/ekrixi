namespace Content.Server._FTL.Wounds;

[RegisterComponent]
public sealed partial class WoundThresholdComponent : Component
{
    [DataField("thresholds")]
    public List<WoundThreshold> Thresholds = new();

    public float TimeSinceLastUpdate = 0;
}

[DataDefinition]
public partial record struct WoundThreshold
{
    [DataField, ViewVariables] public string Wound;
    [DataField, ViewVariables] public string DamageType;
    [DataField, ViewVariables] public float Probability;
    [DataField, ViewVariables] public float Threshold;
}
