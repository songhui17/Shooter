using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections.Generic;

using Shooter;

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
    
    // TODO
    private string _currentScene = "";
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

    private string _tips;
    public string Tips {
        get { return _tips ?? (_tips = ""); }
        set { _tips = value; OnPropertyChanged("Tips"); }
    }

    private static LoadingViewModel _instance;
    public static LoadingViewModel Instance {
        get { return _instance; }
        private set { _instance = value; }
    }

    [SerializeField]
    private float _loadingScreenDuration = 2.0f;
    private float _loadingStartTime = 0.0f;

    public event Action<string> Loaded;
    private void OnLoaded(string level_) {
        if (Loaded != null) {
            Loaded(level_);
        } 
    }

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

                    OnLoaded(_currentScene);
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

    private void SetTips() {
#if UNITY_STANDALONE
            Tips = StringTable.Value("Tips.Standalone");
#else
            Tips = StringTable.Value("Tips.Android");
#endif
    }

    // TODO:
    public void StartFight(string scene_, LevelInfo levelInfo_) {
        LoadingType = LoadingType.StartFight;

        Blackboard.Instance.LastLoadingDone = false;
        _loadingStartTime = Time.realtimeSinceStartup;

        _currentScene = scene_;
        var scene = scene_;
        var level = new Level() {
            ID = (levelInfo_.level_id + 1).ToString(),
            Title = levelInfo_.title,
            Description = "",
            TaskDescriptionList = new List<string>() {
                levelInfo_.task1, levelInfo_.task2, 
                levelInfo_.task3,
            }
        };
        CurrentLevel = level;

        SetTips();

        IsLoading = true;

        _loadingOperation = SceneManager.LoadSceneAsync(scene);
        if (_verbose)
            Debug.Log(string.Format("IsLoading: {0}, scene: {1}",
                                    IsLoading, scene));
    }

    public void BackToLobby(string scene_ = "Lobby"){
        LoadingType = LoadingType.BackToLobby;
        _currentScene = scene_;

        SetTips();

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
