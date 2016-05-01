using UnityEngine;
using UnityEngine.SceneManagement;

public enum LoadingType {
    StartFight = 0,
    BackToLobby,
}

public class LoadingViewModel : ViewModelBase {

    [SerializeField]
    private bool _verbose = true;
    
    private LoadingType _loadingType;
    public LoadingType LoadingType {
        get { return _loadingType; }
        private set { _loadingType = value; }
    }
    
    private Level _currentLevel;
    public Level CurrentLevel {
        get { return _currentLevel; }
        private set { _currentLevel = value; }
    }

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

    private static LoadingViewModel _instance;
    public static LoadingViewModel Instance {
        get { return _instance; }
        private set { _instance = value; }
    }

    [SerializeField]
    private float _loadingScreenDuration = 2.0f;
    private float _loadingStartTime = 0.0f;

    #region MonoBehaviour

    void Awake(){
        if (Instance != null){
            enabled = false;
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Update(){
        if (IsLoading){
            LoadingProgress = _loadingOperation.progress;
            if (_loadingOperation.isDone){
                var elapsedTime = Time.realtimeSinceStartup - _loadingStartTime;
                if (elapsedTime >  _loadingScreenDuration){
                    if (_verbose)
                        Debug.Log("LoadingDone");
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

    #endregion

    #region Methods

    public void StartFight(Level level_){
        LoadingType = LoadingType.StartFight;

        if (_verbose) 
            Debug.Log(string.Format("StartFight level_: {0}", level_));

        CurrentLevel = level_;
        IsLoading = true;
        Blackboard.Instance.LastLoadingDone = false;
        _loadingStartTime = Time.realtimeSinceStartup;

        var scene = level_.Scene;
        _loadingOperation = SceneManager.LoadSceneAsync(scene);
        if (_verbose)
            Debug.Log(string.Format("IsLoading: {0}, scene: {1}",
                                    IsLoading, scene));
    }

    public void BackToLobby(string scene_ = "Lobby"){
        LoadingType = LoadingType.BackToLobby;

        IsLoading = true;
        Blackboard.Instance.LastLoadingDone = false;
        _loadingStartTime = Time.realtimeSinceStartup;
        _loadingOperation = SceneManager.LoadSceneAsync(scene_);
        if (_verbose)
            Debug.Log(string.Format("IsLoading: {0}, scene_: {1}",
                                    IsLoading, scene_));
    }

    #endregion
}
