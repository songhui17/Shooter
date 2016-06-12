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

    private bool _win = true;
    public bool Win {
        get { return _win; }
        set { _win = value; OnPropertyChanged("Win"); }
    }

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
