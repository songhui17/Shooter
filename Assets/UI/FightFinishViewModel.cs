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
