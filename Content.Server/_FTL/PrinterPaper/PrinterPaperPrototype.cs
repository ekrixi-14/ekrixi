using Robust.Shared.Prototypes;

namespace Content.Server._FTL.PrinterPaper;

[Prototype("printerPaper")]
public sealed class PrinterPaperPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// How often will characters be replaced with a %?
    /// </summary>
    [DataField("corruptProb")]
    public float CorruptionProbability { get; } = 0.1f;

    [DataField("corruptChars")]
    public List<string> CorruptionCharacters = new ()
    {
        "%",
        "#",
        ".",
        ",",
        "$",
        "^",
        "&",
        "*",
        "@",
    };

    /// <summary>
    /// The content of the paper
    /// </summary>
    [DataField("content")]
    public string Content { get; } = "boo!";
}
