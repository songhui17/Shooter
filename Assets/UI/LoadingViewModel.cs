using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingViewModel : ViewModelBase {
    [SerializeField]
    private bool _verbose = true;

    private AsyncOperation _loadingOperation;

    private bool _isLoading = false;
    public bool IsLoading {
        get { return _isLoading; }
        set { 
            _isLoading = value;
            OnPropertyChanged("IsLoading");
        }
    }

    private float _loadingProgress = 0;
    public float LoadingProgress {
        get { return _loadingProgress; }
        set {
            _loadingProgress = value;
            OnPropertyChanged("LoadingProgress");
        }
    }

    [SerializeField]
    private float _loadingScreenDuration = 2.0f;
    private float _loadingStartTime = 0.0f;

    public void StartFight(string scene_){
        IsLoading = true;
        Blackboard.Instance.LastLoadingDone = false;
        _loadingStartTime = Time.realtimeSinceStartup;
        _loadingOperation = SceneManager.LoadSceneAsync("Prototype");
        if (_verbose)
            Debug.Log(string.Format("IsLoading: {0}", IsLoading));
    }

    void Update(){
        if (IsLoading){
            LoadingProgress = _loadingOperation.progress;
            if (_loadingOperation.isDone){
                var elapsedTime = Time.realtimeSinceStartup - _loadingScreenDuration;
                if (elapsedTime >  _loadingScreenDuration){
                    _loadingOperation = null;
                    _loadingStartTime = 0.0f;
                    Blackboard.Instance.LastLoadingDone = true;
                    IsLoading = false;
                }
            }
        }else{
            // Dothing
        }
    }
}
