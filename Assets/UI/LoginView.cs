using UnityEngine;
using UnityEngine.UI;

public class LoginView : ViewBase {
    [SerializeField]
    private Animator _loginPanelAnimator;

    [SerializeField]
    private InputField _accountInputField;

    [SerializeField]
    private InputField _passwordInputField;

    [SerializeField]
    private GameObject _statusPanel;

    [SerializeField]
    private Text _statusText;

    [SerializeField]
    private Animator _welcomePanelAnimator;

    void Awake(){
        _accountInputField.onValueChanged.AddListener(value_ => {
            var viewModel = DataContext as LoginViewModel;
            if (viewModel == null) return;
            if (viewModel.Account != value_) {
                viewModel.Account = value_;
            }
        });

        _passwordInputField.onValueChanged.AddListener(value_ => {
            var viewModel = DataContext as LoginViewModel;
            if (viewModel == null) return;
            if (viewModel.Password != value_) {
                viewModel.Password = value_;
            }
        });

        DataContext = gameObject.AddComponent<LoginViewModel>();
    }

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null){
            HandlePropertyChanged(newContext_, "Account");
            HandlePropertyChanged(newContext_, "Password");
            HandlePropertyChanged(newContext_, "ShowLoginPanel");
            HandlePropertyChanged(newContext_, "Message");
            HandlePropertyChanged(newContext_, "ShowStatusPanel");
        }else{
            // TODO: reset UI
        }
    }

    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
        var viewModel = sender_ as LoginViewModel;
        if (viewModel != null) {
            switch (property_ as string){
                case "Account":
                    {
                        if (_accountInputField.text != viewModel.Account) {
                            _accountInputField.text = viewModel.Account;
                        }
                    }
                    break;
                case "Password":
                    {
                        if (_passwordInputField.text != viewModel.Password) {
                            _passwordInputField.text = viewModel.Password;
                        }
                    }
                    break;
                case "ShowLoginPanel":
                    {
                        // _loginPanelAnimator.SetTrigger("toggle");
                        _loginPanelAnimator.SetBool("Open", viewModel.ShowLoginPanel);
                    }
                    break;
                case "ShowStatusPanel":
                    {
                        if (_statusPanel != null) {
                            _statusPanel.SetActive(viewModel.ShowStatusPanel);
                        }
                    }
                    break;
                case "Message":
                    {
                        if (_statusText != null) {
                            if (_statusText.text != viewModel.Message) {
                                _statusText.text = viewModel.Message;
                            }
                        }
                    }
                    break;
                case "Welcome":
                    {
                        if (_welcomePanelAnimator != null){
                            _welcomePanelAnimator.SetTrigger("toggle");
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void OnLoginButton(){
        var viewModel = DataContext as LoginViewModel;
        if (viewModel != null){
            viewModel.Login();
        }
    }

    public void OnEnterButton(){
        var viewModel = DataContext as LoginViewModel;
        if (viewModel != null){
            viewModel.EnterLobby();
        }
    }
}
