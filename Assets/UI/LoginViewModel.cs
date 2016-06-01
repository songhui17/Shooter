using UnityEngine;
using System.Collections;
using Shooter;

public class LoginViewModel : ViewModelBase {
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

    private string _message;
    public string Message {
        get { return _message ?? (_message = ""); }
        set { _message = value; OnPropertyChanged("Message"); }
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
            Message = "正在连接服务器...";
            SockUtil.Instance.ConnectToServer();
        }
        while (true) {
            Debug.Log("Login...");
            yield return waitForSeconds;
            if (SockUtil.Instance.IsConnected) {
                Debug.Log("Send LoginRequest");
                Message = "正在登录帐号...";
                SockUtil.Instance.SendRequest<LoginRequest, LoginRequestResponse>(
                    null, new LoginRequest() {
                        username = Account,
                        password = Password,
                    }, OnLogin);
                break;
            } else if (SockUtil.Instance.IsDisconnected) {
                Debug.Log("Failed to Connect");
                Message = "帐号登录失败";
                _isSendingLogin = false;
                ShowStatusPanel = false;
                SavePlayerPrefs(false);
                break;
            }
        }
    }

    private void OnLogin(LoginRequestResponse response_) {
        Debug.Log(response_);
        _isSendingLogin = false;
        if (response_.errno == 0) {
            Message = "帐号登录成功";
            ShowStatusPanel = false;
            ShowLoginPanel = false;
            SavePlayerPrefs(true);

            OnPropertyChanged("Welcome");
        }
    }

    void Start() {
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
