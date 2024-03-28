using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._FTL.Wounds;

[DataDefinition]
public sealed partial class ToolTreatmentPath : BaseTreatmentPath
{
    public override bool TreatmentCheck(IEntityManager entMan, NetEntity activeHand)
    {
        var currentlyHeld = entMan.GetEntity(activeHand);
        var toolSystem = entMan.System<SharedToolSystem>();
        var quality = IoCManager.Resolve<IPrototypeManager>().Index<ToolQualityPrototype>(ToolQuality.Id);

        return toolSystem.HasQuality(currentlyHeld, quality.ID);
    }

    public override string GetVerbText(WoundComponent currentWound)
    {
        return $"Treat current wound ({currentWound.CurrentTreatmentPath}/{currentWound.TreatmentPaths.Count})";
    }
}
