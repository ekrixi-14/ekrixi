using Robust.Shared.Prototypes;

namespace Content.Server._FTL.FTLPoints;

/// <summary>
/// This is a prototype for getting a specific type of FTL point.
/// </summary>
[Prototype("ftlPoint")]
public sealed class FTLPointPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// Loc string in the FTL menu next to the name ([STAR] Cepheus-I-32).
    /// </summary>
    [DataField("tag")] public string Tag = "";

    /// <summary>
    /// FTL point effects.
    /// </summary>
    [DataField("effects")] public FTLPointEffect[] FtlPointEffects = default!;
}
