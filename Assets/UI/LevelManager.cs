using UnityEngine;
using System.Collections.Generic;

using Shooter;

public class ObservableList<T> : List<T>
{

}

public class LevelManager : ViewModelBase {
    public static LevelManager Instance { get; private set; }

    private List<ActorLevelInfo> _actorLevelInfoList;
    public List<ActorLevelInfo> ActorLevelInfoList
    {
        get { return _actorLevelInfoList ?? (_actorLevelInfoList = new List<ActorLevelInfo>()); }
        set { _actorLevelInfoList = value; OnPropertyChanged("ActorLevelInfoList"); }
    }

    private List<LevelInfo> _levelInfoList;
    public List<LevelInfo> LevelInfoList
    {
        get { return _levelInfoList ?? (_levelInfoList = new List<LevelInfo>()); }
        set { _levelInfoList = value; OnPropertyChanged("LevelInfoList"); }
    }

    void Awake() {
        if (Instance != null){
            enabled = false;
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SockUtil.Instance.RegisterHandler<SpawnBotRequest, SpawnBotRequestResponse> ("spawn_bot", HandleSpawnBotRequest);

        SockUtil.Instance.RegisterHandler<StartLevelRequest, StartLevelRequestResponse>("start_level", HandleStartLevel);

        SockUtil.Instance.RegisterHandler<FinishLevelRequest, FinishLevelRequestResponse>("finish_level", HandleFinishLevel);
    }

    SpawnBotRequestResponse HandleSpawnBotRequest(SpawnBotRequest request_) {
        Debug.Log(request_);
        return new SpawnBotRequestResponse() {
            errno = 0,
        };
    }

    StartLevelRequestResponse HandleStartLevel(StartLevelRequest request_) {
        LoadingViewModel.Instance.Loaded += OnLoaded;
        Debug.LogWarning("TODO");
        LoadingViewModel.Instance.StartFight("Prototype");
        return new StartLevelRequestResponse() { errno = 0 };
    }

    public FightFinishViewModel FightFinish;
    FinishLevelRequestResponse HandleFinishLevel(FinishLevelRequest request_) {
        FightFinish.IsFightFinished = true;
        return new FinishLevelRequestResponse() { errno = 0 };
    }

    void OnLoaded(string scene_) {
        LoadingViewModel.Instance.Loaded -= OnLoaded;
        SockUtil.Instance.SendRequest<EnterLevelRequest, EnterLevelRequestResponse>(
                "enter_level", new EnterLevelRequest(), null);
    }


    public void SetActorLeveInfo(List<ActorLevelInfo> leveInfo_) {
        ActorLevelInfoList = leveInfo_;
    }
    public void SetLevelInfo(List<LevelInfo> leveInfo_) {
        LevelInfoList = leveInfo_;
    }
}
