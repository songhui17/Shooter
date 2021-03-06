using UnityEngine;
using UnityEngine.UI;

public class LoginView : ViewBase {
    // [SerializeField]
    // private Animator _loginPanelAnimator;

    [SerializeField]
    private AnimatedActivate _loginPanelAnimator;

    [SerializeField]
    private GameObject _enterButton;

    [SerializeField]
    private InputField _accountInputField;

    [SerializeField]
    private InputField _passwordInputField;

    [SerializeField]
    private InputField _ipInputField;

    [SerializeField]
    private InputField _portInputField;

    [SerializeField]
    private AnimatedActivate _statusPanel;

    [SerializeField]
    private Text _statusText;

    [SerializeField]
    private Text _welcomeText;

    [SerializeField]
    private Animator _welcomePanelAnimator;

    private ENUM_LOGIN_STATE _prevLoginState;

    [SerializeField]
    private LoginManager _viewModel;

    void Awake(){
        _accountInputField.onValueChanged.AddListener(value_ => {
            var viewModel = DataContext as LoginManager;
            if (viewModel == null) return;
            if (viewModel.Account != value_) {
                viewModel.Account = value_;
            }
        });

        _passwordInputField.onValueChanged.AddListener(value_ => {
            var viewModel = DataContext as LoginManager;
            if (viewModel == null) return;
            if (viewModel.Password != value_) {
                viewModel.Password = value_;
            }
        });

        _ipInputField.onValueChanged.AddListener(value_ => {
            var viewModel = DataContext as LoginManager;
            if (viewModel == null) return;
            if (viewModel.IP != value_) {
                viewModel.IP = value_;
            }
        });

        _portInputField.onValueChanged.AddListener(value_ => {
            var viewModel = DataContext as LoginManager;
            if (viewModel == null) return;
            if (viewModel.Port.ToString() != value_) {
                try{
                    viewModel.Port = int.Parse(value_);
                }catch{
                }
            }
        });

        DataContext = _viewModel;
    }

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null){
            HandlePropertyChanged(newContext_, "Account");
            HandlePropertyChanged(newContext_, "Password");
            HandlePropertyChanged(newContext_, "ShowLoginPanel");

            HandlePropertyChanged(newContext_, "IP");
            HandlePropertyChanged(newContext_, "Port");
            // HandlePropertyChanged(newContext_, "ShowStatusPanel");

            _prevLoginState = (newContext_ as LoginManager).State;
            if (_prevLoginState == ENUM_LOGIN_STATE.Idle) {
                // TODO: EnterState
                _statusPanel.SetActive(false);
            }
            HandlePropertyChanged(newContext_, "State");
        }else{
            // TODO: reset UI
        }
    }

    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
        var viewModel = sender_ as LoginManager;
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
                case "IP":
                    {
                        if (_ipInputField.text != viewModel.IP) {
                            _ipInputField.text = viewModel.IP;
                        }
                    }
                    break;
                case "Port":
                    {
                        if (_portInputField.text != viewModel.Port.ToString()) {
                            _portInputField.text = viewModel.Port.ToString();
                        }
                    }
                    break;
                case "ShowLoginPanel":
                    {
                        // _loginPanelAnimator.SetTrigger("toggle");
                        // _loginPanelAnimator.SetBool("Open", viewModel.ShowLoginPanel);
                        // Debug.LogError("+++++++++++++++" + viewModel.ShowLoginPanel);
                        _loginPanelAnimator.SetActive(viewModel.ShowLoginPanel);
                        _enterButton.SetActive(!viewModel.ShowLoginPanel);
                    }
                    break;
                //case "ShowStatusPanel":
                    //{
                        //if (_statusPanel != null) {
                            //_statusPanel.SetActive(viewModel.ShowStatusPanel);
                        //}
                    //}
                    //break;
                case "State":
                    {
                        switch (viewModel.State) {
                            case ENUM_LOGIN_STATE.Idle:
                                {
                                    if (_prevLoginState != viewModel.State) {
                                        _statusPanel.SetActive(false);
                                        _prevLoginState = viewModel.State;
                                    }
                                }
                                break;
                            case ENUM_LOGIN_STATE.Connecting:
                                {
                                    if (_prevLoginState != viewModel.State) {
                                        _statusPanel.SetActive(true);
                                        _statusText.text = StringTable.Value("Login.Status.Connecting");

                                        _prevLoginState = viewModel.State;
                                    }
                                }
                                break;
                            case ENUM_LOGIN_STATE.Logining:
                                {
                                    if (_prevLoginState != viewModel.State) {
                                        _statusPanel.SetActive(true);
                                        _statusText.text = StringTable.Value("Login.Status.Logining");
                                        _prevLoginState = viewModel.State;
                                    }
                                }
                                break;
                            case ENUM_LOGIN_STATE.ConnectFailed:
                                {
                                    if (_prevLoginState != viewModel.State) {
                                        _statusText.text = StringTable.Value("Login.Status.ConnectFailed");
                                        _statusPanel.SetActiveAnimated(false, () => {
                                            viewModel.ShowLoginPanel = true;
                                        });
                                        _prevLoginState = viewModel.State;
                                    }
                                }
                                break;
                            case ENUM_LOGIN_STATE.LoginSuccess:
                                {
                                    if (_prevLoginState != viewModel.State) {
                                        _statusText.text = StringTable.Value("Login.Status.LoginSuccess");
                                        _statusPanel.SetActive(false);
                                        _prevLoginState = viewModel.State;
                                    }
                                }
                                break;
                            case ENUM_LOGIN_STATE.LoginFailed:
                                {
                                    if (_prevLoginState != viewModel.State) {
                                        _statusText.text = StringTable.Value("Login.Status.LoginFailed");
                                        _statusPanel.SetActiveAnimated(false, () => {
                                            viewModel.ShowLoginPanel = true;
                                        });
                                        _prevLoginState = viewModel.State;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "Welcome":
                    {
                        if (_welcomePanelAnimator != null){
                            _welcomePanelAnimator.SetTrigger("toggle");
                            var account = viewModel.ShortName;
                            _welcomeText.text = string.Format(StringTable.Value("Login.Welcome.Format"), account);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void OnLoginButton(){
        var viewModel = DataContext as LoginManager;
        if (viewModel != null){
            viewModel.Login();
        }
    }

    public void OnEnterButton(){
        var viewModel = DataContext as LoginManager;
        if (viewModel != null){
            viewModel.EnterLobby();
        }
    }
}
