using System.Linq;
using Content.Server._FTL.FTLPoints.Prototypes;
using Content.Server._FTL.FTLPoints.Systems;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed;

namespace Content.Server._FTL.FTLPoints.Commands;

[ToolshedCommand, AdminCommand(AdminFlags.Mapping)]
public sealed class GeneratePointCommand : ToolshedCommand
{
    [Dependency] private readonly EntityManager _entityManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    [CommandImplementation("id")]
    public void GenerateWithId(
        [CommandInvocationContext] IInvocationContext ctx,
        [PipedArgument] string id
    )
    {
        if (!_prototypeManager.TryIndex<FTLPointPrototype>(id, out var prototype))
        {
            ctx.WriteLine("Invalid ID.");
            return;
        }
        _entityManager.System<FTLPointsSystem>().GenerateDisposablePoint(prototype);
        ctx.WriteLine("Generated FTL point.");
    }

    [CommandImplementation]
    public void GenerateRandom([CommandInvocationContext] IInvocationContext ctx)
    {
        _entityManager.System<FTLPointsSystem>().GenerateDisposablePoint();
        ctx.WriteLine("Generated random FTL point.");
    }
}
