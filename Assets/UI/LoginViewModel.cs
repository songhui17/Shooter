using UnityEngine;

public class LoginViewModel : ViewModelBase {
    public void Login(){
        var loadingViewModel = LoadingViewModel.Instance;
        if (loadingViewModel != null)
            loadingViewModel.BackToLobby();
    }
}
