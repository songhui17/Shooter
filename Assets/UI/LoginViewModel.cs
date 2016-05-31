using UnityEngine;
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

    public void Login(){
        var loadingViewModel = LoadingViewModel.Instance;
        // if (loadingViewModel != null)
            // loadingViewModel.BackToLobby();
        SockUtil.Instance.SendRequest<LoginRequest, LoginRequestResponse>(
            null, new LoginRequest() {
                username = Account,
                password = Password,
            }, OnLogin);
    }

    private void OnLogin(LoginRequestResponse response_) {
        Debug.Log(response_);
    }
}
