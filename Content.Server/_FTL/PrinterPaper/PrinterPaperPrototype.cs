using Robust.Shared.Prototypes;

namespace Content.Server._FTL.PrinterPaper;

[Prototype("printerPaper")]
public sealed class PrinterPaperPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// The content of the paper
    /// </summary>
    [DataField("content")]
    public string Content { get; } = "boo!";
}
