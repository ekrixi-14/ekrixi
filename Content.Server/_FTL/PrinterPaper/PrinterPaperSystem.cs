using System.Linq;
using System.Text;
using Content.Server.Paper;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._FTL.PrinterPaper;

public sealed class PrinterPaperSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RandomPrinterPaperComponent, ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(EntityUid uid, RandomPrinterPaperComponent component, ComponentInit args)
    {
        var prototypes = _prototypeManager.EnumeratePrototypes<PrinterPaperPrototype>().ToList();
        PrinterPaperPrototype? prototype = null;

        while (prototype == null)
        {
            var chosenPrototype = _random.Pick(prototypes);
            if (component.Tag == null)
            {
                prototype = chosenPrototype;
                break;
            }
            if (component.Tag == chosenPrototype.Tag)
                prototype = chosenPrototype;
        }

        var contentPicked = prototype.Content;
        var content = new StringBuilder();

        foreach (var t in contentPicked)
        {
            if (char.IsWhiteSpace(t))
                content.Append(t);
            else
                content.Append(_random.Prob(prototype.CorruptionProbability) ? _random.Pick(prototype.CorruptionCharacters) : t);
        }

        component.Content = content.ToString();
        EnsureComp<PaperComponent>(uid);
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
}
