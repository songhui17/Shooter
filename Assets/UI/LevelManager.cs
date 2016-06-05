using UnityEngine;
using System;
using System.Collections.Generic;

using Shooter;

using Vector3 = UnityEngine.Vector3;

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

    [SerializeField]
    private GameObject _spiderPrefab;

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
        switch (request_.bot_type) {
            case "spider":
                {
                    var position = new Vector3() {
                        x = request_.position.x,
                        y = request_.position.y,
                        z = request_.position.z,
                    };
                    var rotation = Quaternion.Euler(0, request_.rotation, 0);
                    var spider = GameObject.Instantiate(
                        _spiderPrefab, position, rotation) as GameObject;
                    var spiderBot = spider.GetComponent<Bot>();
                    spiderBot.BotKilled += (bot_) => {
                        SockUtil.Level0BotKilled(new Level0BotKilledRequest(), null);
                    };
                    return new SpawnBotRequestResponse() {
                        errno = 0,
                    };
                }
                break;
            default:
                {
                    // TODO
                    var info = "Invalid bot_type:" + request_.bot_type;
                    Debug.LogError(info);
                    throw new Exception(info);
                }
                break;
        }
    }

    StartLevelRequestResponse HandleStartLevel(StartLevelRequest request_) {
        LoadingViewModel.Instance.Loaded += OnLoaded;
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
        SockUtil.EnterLevel(new EnterLevelRequest(), null);
    }


    public void SetActorLeveInfo(List<ActorLevelInfo> leveInfo_) {
        // ActorLevelInfoList = leveInfo_;
        foreach(var info in leveInfo_) {
            var prev = ActorLevelInfoList.FindIndex(info2_ => info2_.level_id == info.level_id);
            if (prev >= 0) {
                ActorLevelInfoList[prev] = info;
            }else{
                ActorLevelInfoList.Add(info);
            }
        }
        ActorLevelInfoList = ActorLevelInfoList;
    }
    public void SetLevelInfo(List<LevelInfo> leveInfo_) {
        LevelInfoList = leveInfo_;
    }
}
