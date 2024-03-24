using Content.Shared.Damage;

namespace Content.Shared._FTL.Wounds;

[RegisterComponent]
public sealed partial class WoundComponent : Component
{
    [DataField, ViewVariables] public DamageSpecifier Damage;
    [DataField] public LocId WoundExamineMessage;
}
