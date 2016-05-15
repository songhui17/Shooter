using UnityEngine;
using UnityEngine.UI;

public class LoginView : ViewBase {
    void Awake(){
        DataContext = gameObject.AddComponent<LoginViewModel>();
    }

    public void OnLoginButton(){
        var viewModel = DataContext as LoginViewModel;
        if (viewModel != null){
            viewModel.Login();
        }
    }
}
