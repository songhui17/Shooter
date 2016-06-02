using UnityEngine;

using Shooter;

public class FightFinishViewModel : ViewModelBase {
    private bool _isFightFinished = false;
    public bool IsFightFinished {
        get { return _isFightFinished; }
        set {
            _isFightFinished = value;
            if (value) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
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
    //
    void Awake() {
        LevelManager.Instance.FightFinish = this;
    }

    public void BackToLobby(){
        if (LoadingViewModel.Instance != null){
            SockUtil.Instance.SendRequest<LeaveLevelRequest, LeaveLevelRequestResponse>(
                "leave_level", new LeaveLevelRequest(), null);

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
