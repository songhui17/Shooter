using UnityEngine;

public class FightFinishViewModel : ViewModelBase {
    private bool _isFightFinished = false;
    public bool IsFightFinished {
        get { return _isFightFinished; }
        set {
            _isFightFinished = value;
            OnPropertyChanged("IsFightFinished");
        }
    }

    // private static FightFinishViewModel _instance;
    // public static FightFinishViewModel Instance {
    //     get { return _instance; }
    //     private set { _instance = value; }
    // }

    // void Awake(){
    //     if (Instance != null){
    //         enabled = false;
    //         Destroy(gameObject);
    //         return;
    //     }
    //     Instance = this;

    //     DontDestroyOnLoad(gameObject);
    // }

    public void BackToLobby(){
        if (LoadingViewModel.Instance != null){
            LoadingViewModel.Instance.BackToLobby("Lobby");
        }else{
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
