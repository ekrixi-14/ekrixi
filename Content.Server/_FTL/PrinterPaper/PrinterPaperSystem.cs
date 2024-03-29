using System.Linq;
using Content.Server.Paper;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._FTL.PrinterPaper;

public sealed class PrinterPaperSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<RandomPrinterPaperComponent, PaperComponent>();
        while (query.MoveNext(out _, out var randomPaper, out var paper))
        {
            paper.Content = randomPaper.Content; // to prevent writing on paper
        }

        base.Update(frameTime);
    }
}
