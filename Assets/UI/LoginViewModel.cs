using UnityEngine;
using System.Collections;
using Shooter;


public enum ENUM_LOGIN_STATE {
    Idle,
    Connecting,
    Logining,
    ConnectFailed,
    LoginSuccess,
    LoginFailed,
}

public class LoginViewModel : ViewModelBase {
    public static LoginViewModel Instance {
        get; private set;
    }

    private string _account;
    public string Account {
        get { return _account ?? (_account = ""); }
        set { _account = value; OnPropertyChanged("Account"); }
    }

    private string _password;
    public string Password {
        get { return _password ?? (_password = ""); }
        set { _password = value; OnPropertyChanged("Password"); }
    }

    private bool _showLoginPanel;
    public bool ShowLoginPanel {
        get { return _showLoginPanel; }
        set { _showLoginPanel = value; OnPropertyChanged("ShowLoginPanel"); }
    }


    // TODO: 
    private bool _showStatusPanel;
    public bool ShowStatusPanel {
        get { return _showStatusPanel; }
        set { _showStatusPanel = value; OnPropertyChanged("ShowStatusPanel"); }
    }

    private ENUM_LOGIN_STATE _state = ENUM_LOGIN_STATE.Idle;
    public ENUM_LOGIN_STATE State {
        get { return _state; }
        set { _state = value; OnPropertyChanged("State"); }
    }

    private bool _isSendingLogin = false;
    public void Login(){
        if (_isSendingLogin) {
            Debug.Log("_isSendingLogin: true # TODO");
            return;
        }

        _isSendingLogin = true;
        StartCoroutine(CoLogin());
        // var loadingViewModel = LoadingViewModel.Instance;
    }

    public void EnterLobby() {
        LoadingViewModel.Instance.BackToLobby();
    }

    IEnumerator CoLogin() {
        var waitForSeconds = new WaitForSeconds(0.5f);
        ShowStatusPanel = true;
        if (!SockUtil.Instance.IsConnected) {
            State = ENUM_LOGIN_STATE.Connecting;
            SockUtil.Instance.ConnectToServer();
        }
        while (true) {
            Debug.Log("Login...");
            yield return waitForSeconds;
            if (SockUtil.Instance.IsConnected) {
                Debug.Log("Send LoginRequest");
                State = ENUM_LOGIN_STATE.Logining;
                var request = new LoginRequest() {
                    username = Account,
                    password = Password,
                };
                SockUtil.Login(request, OnLogin);
                break;
            } else if (SockUtil.Instance.IsConnectFailed) {
                Debug.Log("Failed to Connect");
                State = ENUM_LOGIN_STATE.ConnectFailed;
                _isSendingLogin = false;
                // ShowStatusPanel = false;
                // ShowLoginPanel = true;
                SavePlayerPrefs(false);
                break;
            }
        }
    }

    private void OnLogin(LoginRequestResponse response_, int requestId_) {
        Debug.Log(response_);
        _isSendingLogin = false;
        if (response_.errno == 0) {
            State = ENUM_LOGIN_STATE.LoginSuccess;
            ShowStatusPanel = false;
            ShowLoginPanel = false;
            SavePlayerPrefs(true);

            OnPropertyChanged("Welcome");
        }else{
            State = ENUM_LOGIN_STATE.LoginFailed;
            ShowStatusPanel = false;
            ShowLoginPanel = true;
            SavePlayerPrefs(false);
        }
    }

    void Start() {
        if (Instance != null) {
            enabled = false;
            Destroy(gameObject);
            Debug.LogWarning("Multiple instance of LoginViewModel");
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Account = PlayerPrefs.GetString("Account");
        Password = PlayerPrefs.GetString("Password");
        var validAccount = PlayerPrefs.GetInt("ValidAccount", 0) == 1;
        if (validAccount && Account != ""){
            Debug.Log("Automatically use prev account to login");
            ShowLoginPanel = false;
            Login();
        }else {
            ShowLoginPanel = true;
        }
        var sockutil = SockUtil.Instance;
    }

    void SavePlayerPrefs(bool valid_) {
        if (valid_) {
            PlayerPrefs.SetString("Account", Account);
            PlayerPrefs.SetString("Password", Password);
            PlayerPrefs.SetInt("ValidAccount", 1);
        }else{
            PlayerPrefs.SetString("Account", Account);
            PlayerPrefs.SetString("Password", Password);
            PlayerPrefs.SetInt("ValidAccount", 0);
        }
    }
}
