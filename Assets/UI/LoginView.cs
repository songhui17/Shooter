using UnityEngine;
using UnityEngine.UI;

public class LoginView : ViewBase {
    [SerializeField]
    private InputField _accountInputField;

    [SerializeField]
    private InputField _passwordInputField;

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
            HandlePropertyChanged(newContext_, "Name");
            HandlePropertyChanged(newContext_, "Password");
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
}
