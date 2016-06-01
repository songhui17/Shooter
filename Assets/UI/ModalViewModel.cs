using UnityEngine;

public class ModalViewModel : ViewModelBase {
    private bool _show;
    public bool Show {
        get { return _show; }
        set { _show = value; OnPropertyChanged("Show"); }
    }

    private string _message;
    public string Message {
        get { return _message ?? (_message = ""); }
        set { _message = value; OnPropertyChanged("Message"); }
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

    public void ShowMessage(string message_) {
        Message = message_;
        Show = true;
    }

    public void Hide() {
        Show = false;
    }
}
