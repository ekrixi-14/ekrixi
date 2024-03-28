namespace Content.Server._FTL.PrinterPaper;

/// <summary>
/// This is used for setting the content of paper on spawn to a random prototype. Also makes the paper readonly
/// </summary>
[RegisterComponent]
public sealed partial class RandomPrinterPaperComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly)] public string Content = "";
}
