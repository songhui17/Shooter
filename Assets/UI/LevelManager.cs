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

    //TODO:
    private int _currentLevelId = -1;
    private Level2 _level2;

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
            case "spider_remote":
            case "spider_king":
                {
                    //TODO:
                    if (_level2 != null) {
                        return _level2.HandleSpawnBotRequest(request_);
                    }else{
                        var info = "_level2 is null";
                        Debug.LogError(info);
                        throw new Exception(info);
                    }
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
        Debug.Log("HandleStartLevel:" + request_);
        _currentLevelId = request_.level_id;
        //TODO:??
        FightFinish = null;
        LoadingViewModel.Instance.Loaded += OnLoaded;
        LoadingViewModel.Instance.StartFight("Prototype", LevelInfoList[_currentLevelId]);
        return new StartLevelRequestResponse() { errno = 0 };
    }

    public FightFinishViewModel FightFinish;
    FinishLevelRequestResponse HandleFinishLevel(FinishLevelRequest request_) {
        FightFinish.Win = request_.win;

        // FightFinish.IsFightFinished = true;
        var actorInfo = ActorManager.Instance.ActorInfo;
        var actorObj = GameObject.FindGameObjectsWithTag("Player")[0];
        var actor = actorObj.GetComponent<Bot>();
        // actorInfo.max_hp = actor.MaxHP;
        actorInfo.hp = actor.HP;
        var updateHpRequest = new UpdateActorHpRequest() {
            actor_id = actorInfo.actor_id,
            hp = actorInfo.hp,
            max_ammo = actorInfo.max_ammo,
            ammo = actorInfo.ammo,
        };
        SockUtil.UpdateActorHp(updateHpRequest, OnUpdateActorHp);

        //TODO
        _level2 = null;
        return new FinishLevelRequestResponse() { errno = 0 };
    }

    void OnUpdateActorHp(UpdateActorHpRequestResponse response_, int requestId_) {
        Debug.Log(response_);
        if (response_.errno == (int)ENUM_SHOOTER_ERROR.E_OK) {
            FightFinish.IsFightFinished = true;
        }else{
            ModalViewModel.Instance.ShowMessage(
                StringTable.Value("Actor.UpdateActorHp.Failed"),
                autoHide_:true, onHide_:() => {
                    FightFinish.IsFightFinished = true;
                });
        }
    }

    void OnLoaded(string scene_) {
        LoadingViewModel.Instance.Loaded -= OnLoaded;
        SockUtil.EnterLevel(new EnterLevelRequest(), null);

        // TODO:
        var actorObj = GameObject.FindGameObjectsWithTag("Player")[0];
        var actor = actorObj.GetComponent<Bot>();
        var actorInfo = ActorManager.Instance.ActorInfo;
        actor.MaxHP = actorInfo.max_hp;
        actor.HP = actorInfo.hp;
        actor.DestroyOnDead = false;
        actor.BotKilled += (actor_) => {
            // magic number -2
            SockUtil.Level0BotKilled(new Level0BotKilledRequest() { bot_id = -2 }, null);
        };

        switch (_currentLevelId) {
            case 1:
                {
                    var level2Obj = new GameObject("Level2");
                    var level2 = level2Obj.AddComponent<Level2>();
                    //TODO
                    _level2 = level2;
                }
                break;
            default:
                break;
        }
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

    public bool LevelFinished {
        get {
            if (FightFinish != null) {
                return FightFinish.IsFightFinished;
            }else{
                return false;
            }
        }
    }
}
