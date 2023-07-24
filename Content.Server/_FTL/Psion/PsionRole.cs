using Content.Server.Roles;
using Content.Shared.Roles;

namespace Content.Server._FTL.Psion;

public sealed class PsionRole : AntagonistRole
{
    public PsionRole(Mind.Mind mind, AntagPrototype antagPrototype) : base(mind, antagPrototype) { }
}
