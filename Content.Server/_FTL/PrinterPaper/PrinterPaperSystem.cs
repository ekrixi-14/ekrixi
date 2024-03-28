using System.Linq;
using Content.Server.Paper;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._FTL.PrinterPaper;

public sealed class PrinterPaperSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<RandomPrinterPaperComponent, ComponentInit>(OnInit);
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<RandomPrinterPaperComponent, PaperComponent>();
        while (query.MoveNext(out _, out var randomPaper, out var paper))
        {
            paper.Content = randomPaper.Content; // to prevent writing on paper
        }

        base.Update(frameTime);
    }

    private void OnInit(EntityUid uid, RandomPrinterPaperComponent component, ComponentInit args)
    {
        var paper = EnsureComp<PaperComponent>(uid);
        var prototypes = _prototypeManager.EnumeratePrototypes<PrinterPaperPrototype>().ToList();
        var content = _random.Pick(prototypes).Content;
        paper.Content = content;
        component.Content = content;
    }
}
