using Content.Client.Message;
using Content.Client.UserInterface.Controls;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._FTL.Economy;

[GenerateTypedNameReferences]
public sealed partial class PdaAtmUiWindow : FancyWindow
{
    public event Action<int>? OnWithdrawRequest;
    public event Action? OnDepositRequest;
    public event Action? OnLockRequest;
    public event Action<string>? OnUnlockRequest;
    public event Action<string>? OnPinChangeRequest;
    public int Amount;

    public PdaAtmUiWindow(IdAtmBoundUserInterface ui, EntityUid entity)
    {
        RobustXamlLoader.Load(this);
        AmountEdit.OnValueChanged += val =>
        {
            Amount = val;
        };
        WithdrawButton.OnPressed += _ =>
        {
            WithdrawMenu.Visible = false;
            ConfirmWithdrawMenu.Visible = true;
            ConfirmWithdrawalMessage.SetMarkup(Loc.GetString("credit-app-ui-withdraw-review-message", ("credits", Amount)));
        };
        ConfirmWithdrawButton.OnPressed += _ =>
        {
            ConfirmWithdrawMenu.Visible = false;
            SuccessMenu.Visible = true;
            OnWithdrawRequest?.Invoke(Amount);
        };

        // cancel
        CancelDeposit.OnPressed += _ =>
        {
            DepositMenu.Visible = false;
            RequestMenu.Visible = true;
        };
        CancelWithdrawal.OnPressed += _ =>
        {
            WithdrawMenu.Visible = false;
            RequestMenu.Visible = true;
        };
        ConfirmCancelWithdrawal.OnPressed += _ =>
        {
            WithdrawMenu.Visible = true;
            ConfirmWithdrawMenu.Visible = false;
        };

        // request
        RequestDeposit.OnPressed += _ =>
        {
            DepositMenu.Visible = true;
            RequestMenu.Visible = false;
        };
        RequestWithdraw.OnPressed += _ =>
        {
            WithdrawMenu.Visible = true;
            RequestMenu.Visible = false;
        };
        RequestLock.OnPressed += _ =>
        {
            OnLockRequest?.Invoke();
        };
        RequestUnlock.OnPressed += _ =>
        {
            OnUnlockRequest?.Invoke(PinEdit.Text);
            PinEdit.Clear();
        };
        CPinChange.OnPressed += _ =>
        {
            OnPinChangeRequest?.Invoke(CPinEdit.Text);
            CPinScreen.Visible = false;
            SuccessMenu.Visible = true;
            CPinEdit.Clear();
        };
        RequestPinChange.OnPressed += _ =>
        {
            CPinScreen.Visible = true;
            RequestMenu.Visible = false;
        };

        // deposit
        DepositButton.OnPressed += _ =>
        {
            DepositMenu.Visible = false;
            SuccessMenu.Visible = true;
            OnDepositRequest?.Invoke();
        };

        // success
        CloseSuccessMenu.OnPressed += _ =>
        {
            RequestMenu.Visible = true;
            SuccessMenu.Visible = false;
        };

        // text and other
        DepositWarning.SetMarkup(Loc.GetString("credit-app-ui-deposit-review-message"));
        NoIdCardMessage.SetMarkup(Loc.GetString("credit-app-ui-error-no-id"));
        LockedIdCardMessage.SetMarkup(Loc.GetString("credit-app-ui-error-card-locked"));
        SuccessMenuLabel.SetMarkup(Loc.GetString("credit-app-ui-generic-success"));
        CPinMessage.SetMarkup(Loc.GetString("credit-app-ui-change-pin-review-message"));
        ConfirmCancelWithdrawal.AddStyleClass("ButtonColorRed");
        CancelWithdrawal.AddStyleClass("ButtonColorRed");
        CancelDeposit.AddStyleClass("ButtonColorRed");

        // change pin screen
        CPinEdit.IsValid += text =>
        {
            if (int.TryParse(text, out _))
            {
                return true;
            }
            return false;
        };
        CPinEdit.OnTextChanged += text =>
        {
            CPinChange.Disabled = text.Text.Length != 4;
        };

        // lock screen
        PinEdit.IsValid += text =>
        {
            if (int.TryParse(text, out _))
            {
                return true;
            }
            return false;
        };
        PinEdit.OnTextChanged += text =>
        {
            RequestUnlock.Disabled = text.Text.Length != 4;
        };
        RequestUnlock.Disabled = true;
    }

    public void SetWelcomeMessage(string name, int credits)
    {
        WelcomeMessage.SetMarkup(Loc.GetString("credit-app-ui-welcome-message", ("name", name), ("credits", credits)));
    }

    public void SetDisabledWithdrawButton(bool disabled)
    {
        RequestWithdraw.Disabled = disabled;
    }

    public void SetDisabledDepositButton(bool disabled)
    {
        RequestDeposit.Disabled = disabled;
    }

    public void SetCurrentScreen(CurrentUIScreen state)
    {
        switch (state)
        {
            case CurrentUIScreen.Locked:
                NoIdCard.Visible = false;
                PinScreen.Visible = true;
                WithIdCard.Visible = false;
                break;
            case CurrentUIScreen.Transaction:
                NoIdCard.Visible = false;
                PinScreen.Visible = false;
                WithIdCard.Visible = true;
                break;
            default:
                NoIdCard.Visible = true;
                PinScreen.Visible = false;
                WithIdCard.Visible = false;
                break;
        }
    }

    public void SetMaxValueSlider(int maxValue)
    {
        AmountEdit.MaxValue = maxValue;
    }

    public enum CurrentUIScreen
    {
        NoID,
        Locked,
        Transaction
    }
}
