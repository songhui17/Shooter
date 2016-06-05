using UnityEngine;

public class ModalViewModel : ViewModelBase {
    private bool _show;
    public bool Show {
        get { return _show; }
        set { _show = value; OnPropertyChanged("Show"); }
    }
    public bool AutoHide { get; private set; }

    private string _message;
    public string Message {
        get { return _message ?? (_message = ""); }
        set { _message = value; OnPropertyChanged("Message"); }
    }

    private bool _showSpinner;
    public bool ShowSpinner {
        get { return _showSpinner; }
        set { _showSpinner = value; OnPropertyChanged("ShowSpinner"); }
    }

    public static ModalViewModel Instance { get; private set; }
    void Awake() {
        if (Instance != null){
            enabled = false;
            Destroy(gameObject);
            Debug.LogWarning("Multiple instance of ModalViewModel");
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowMessage(string message_, bool autoHide_=false) {
        Message = message_;
        AutoHide = autoHide_;
        Show = true;
    }

    public void Hide() {
        Show = false;
    }
}
