using Content.Server.Actions;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Shared._FTL.Psion;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Store;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server._FTL.Psion;

public sealed class PsionSystem : SharedPsionSystem
{
    [Dependency] private readonly StoreSystem _storeSystem = default!;
    [Dependency] private readonly ActionsSystem _actionsSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<PsionComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<PsionComponent, PsionShopActionEvent>(OnShop);
    }

    private void OnStartup(EntityUid uid, PsionComponent component, ComponentStartup args)
    {
        var shopAction = new InstantAction(_prototypeManager.Index<InstantActionPrototype>("PsionShop"));
        _actionsSystem.AddAction(uid, shopAction, null);

        var store = AddComp<StoreComponent>(uid);
        store.Categories.Add("PsionAbilities");
        store.CurrencyWhitelist.Add("ObtainedPower");

        // todo figure out how to make psion shop work
        var bui = AddComp<ServerUserInterfaceComponent>(uid);
    }

    private void OnShop(EntityUid uid, PsionComponent component, PsionShopActionEvent args)
    {
        if (!TryComp<StoreComponent>(uid, out var store))
            return;
        _storeSystem.ToggleUi(uid, uid, store);
        Log.Debug("A");
    }
}
