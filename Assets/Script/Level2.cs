using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Shooter;
using Vector3 = UnityEngine.Vector3;


public class TaskInfo : DataModelBase {
    private ActorLevelInfo _actorLevelInfo;
    public ActorLevelInfo ActorLevelInfo {
        get {
            return _actorLevelInfo ?? (_actorLevelInfo = new ActorLevelInfo());
        }
        set {
            _actorLevelInfo = value; OnPropertyChanged("ActorLevelInfo");
        }
    }

    protected string _task1Info;
    public virtual string Task1Info {
        get { return _task1Info ?? (_task1Info = ""); }
        set { _task1Info = value; OnPropertyChanged("Task1Info"); }
    }

    protected string _task2Info;
    public virtual string Task2Info {
        get { return _task2Info ?? (_task2Info = ""); }
        set { _task2Info = value; OnPropertyChanged("Task2Info"); }
    }

    protected string _task3Info;
    public virtual string Task3Info {
        get { return _task3Info ?? (_task3Info = ""); }
        set { _task3Info = value; OnPropertyChanged("Task3Info"); }
    }

    public LevelInfo LevelInfo {
        set {
            _task1Info = value.task1;
            _task2Info = value.task2;
            _task3Info = value.task3;
        }
    }
}

public class Level2TaskInfo : TaskInfo {
    private int _tripleKill = 0;
    public int TripleKill { 
        get { return _tripleKill; }
        set { _tripleKill = value; OnPropertyChanged("Task1Info"); }
    }

    private float _towerHp = 1;
    public float TowerHp {
        get { return _towerHp; }
        set { _towerHp = value; OnPropertyChanged("Task3Info"); }
    }

    public override string Task1Info {
        get {
            if (_task1Info == null) return "";
            else{
                return string.Format("{0} ({1}/1)", _task1Info, TripleKill);
            }
        }
        set { _task1Info = value; OnPropertyChanged("Task1Info"); }
    }

    public override string Task3Info {
        get {
            if (_task3Info == null) return "";
            else{
                return string.Format("{0} ({1}%)", _task3Info, (int)(TowerHp * 100));
            }
        }
        set { _task3Info = value; OnPropertyChanged("Task3Info"); }
    }
}

public interface ILevel {
    TaskInfo GetTaskInfo();
    SpawnBotRequestResponse HandleSpawnBotRequest(SpawnBotRequest request_);
}

public class Level2 : MonoBehaviour, ILevel {
    private List<BotRemote> _botList = new List<BotRemote>();

    private GameObject _spiderRemotePrefab;
    private GameObject SpiderRemotePrefab {
        get {
            return _spiderRemotePrefab ?? (
                _spiderRemotePrefab = Resources.Load<GameObject>("Spider_Remote"));
        }
    }
    private GameObject _spiderKingPrefab;
    private GameObject SpiderKingPrefab {
        get {
            return _spiderKingPrefab ?? (
                // _spiderKingPrefab = Resources.Load<GameObject>("Spider_Remote"));
                _spiderKingPrefab = Resources.Load<GameObject>("Spider_King"));
        }
    }

    private GameObject _ammoItemPrefab;
    private GameObject AmmoItemPrefab {
        get {
            return _ammoItemPrefab ?? (
                _ammoItemPrefab = Resources.Load<GameObject>("Ammo_Item"));
        }
    }

    public Level2TaskInfo TaskInfo = new Level2TaskInfo();
    public TaskInfo GetTaskInfo() {
        return TaskInfo;
    }

    private Transform _playerTransform;

    void Awake() {
        TaskInfo.LevelInfo = LevelManager.Instance.CurrentLevelInfo;

        SockUtil.Instance.RegisterHandler<BotTransformSyncRequest, BotTransformSyncRequestResponse>("bot_transform_sync", HandleBotTransformSyncRequest, force_:true);
        SockUtil.Instance.RegisterHandler<BotExploseRequest, BotExploseRequestResponse>("bot_explose", HandleBotExploseRequest, force_:true);
        SockUtil.Instance.RegisterHandler<TowerHpSyncRequest, TowerHpSyncRequestResponse>("tower_hp_sync", HandleTowerHpSyncRequest, force_:true);
        SockUtil.Instance.RegisterHandler<SpawnItemRequest, SpawnItemRequestResponse>("spawn_item", HandleSpawnItemRequest, force_:true);
        SockUtil.Instance.RegisterHandler<UseItemRequest, UseItemRequestResponse>("use_item", HandleUseItemRequest, force_:true);
        SockUtil.Instance.RegisterHandler<BotPlayAnimationRequest, BotPlayAnimationRequestResponse>("bot_play_animation", HandleBotPlayAnimationRequest, force_:true);
        SockUtil.Instance.RegisterHandler<ActorLevelInfoSyncRequest, ActorLevelInfoSyncRequestResponse>("actor_level_info_sync", HandleActorLevelInfoSyncRequest, force_:true);
        SockUtil.Instance.RegisterHandler<KillReportSyncRequest, KillReportSyncRequestResponse>("kill_report_sync", HandleKillReportSyncRequest, force_:true);
    }

    IEnumerator Start() {
        var wait = new WaitForSeconds(0.1f);
        var player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        var transformSyncRequest = new BotTransformSyncRequest() {
            bot_id = -2,
            position = new Shooter.Vector3(),
        };
        while(true) {
            yield return wait;
            var position = player.position;
            transformSyncRequest.position.x = position.x;
            transformSyncRequest.position.y = position.y;
            transformSyncRequest.position.z = position.z;
            SockUtil.BotTransformSync(transformSyncRequest, null); 
        }
    }

    ActorLevelInfoSyncRequestResponse HandleActorLevelInfoSyncRequest(ActorLevelInfoSyncRequest request_) {
        Debug.Log(request_);
        TaskInfo.ActorLevelInfo = request_.actor_level_info;
        return new ActorLevelInfoSyncRequestResponse();
    }

    KillReportSyncRequestResponse HandleKillReportSyncRequest(KillReportSyncRequest request_) {
        Debug.Log(request_);
        TaskInfo.TripleKill = request_.kill_report.triple_kill;
        return new KillReportSyncRequestResponse();
    }

    BotPlayAnimationRequestResponse HandleBotPlayAnimationRequest(BotPlayAnimationRequest request_) {
        Debug.Log(request_);
        var bot = _botList.Find(bot_ => bot_.ID == request_.bot_id);
        if (bot != null) {
            bot.PlayAnimation(request_.animation_clip);
        }
        return new BotPlayAnimationRequestResponse();
    }

    UseItemRequestResponse HandleUseItemRequest(UseItemRequest request_) {
        Debug.Log(request_);
        switch (request_.item_type) {
            case "ammo":
                {
                    ActorManager.Instance.MaxAmmo += 18;
                    _ammoItem.SetActive(false);
                    _ammoItem = null;
                }
                break;
            default:
                break;
        }
        return new UseItemRequestResponse();
    }

    private GameObject _ammoItem;
    SpawnItemRequestResponse HandleSpawnItemRequest(SpawnItemRequest request_) {
        Debug.Log(request_);
        switch (request_.item_type) {
            case "ammo":
                {
                    var ammoItemObj = GameObject.Instantiate(AmmoItemPrefab);
                    var ammoTransform = ammoItemObj.transform;
                    var position = request_.position;
                    ammoTransform.position = new Vector3() {
                        x = position.x,
                        y = position.y,
                        z = position.z,
                    };
                    _ammoItem = ammoItemObj;
                }
                break;
            default:
                break;
        }
        return new SpawnItemRequestResponse();
    }

    BotTransformSyncRequestResponse HandleBotTransformSyncRequest(BotTransformSyncRequest request_) {
        var bot = _botList.Find(bot_ => bot_.ID == request_.bot_id);
        if (bot != null) {
            // var info = "HandleBotTransformSyncRequest:\n";
            // info += request_;
            // Debug.Log(info);

            var position = request_.position;
            var angle = request_.rotation;

            bot.transform.position = new Vector3() {
                x = position.x,
                y = position.y,
                z = position.z,
            };
            bot.transform.rotation = Quaternion.Euler(0, angle, 0);

            if (request_.waypoint_position != null) {
                Debug.DrawLine(bot.transform.position, new Vector3() {
                    x = request_.waypoint_position.x,
                    y = request_.waypoint_position.y,
                    z = request_.waypoint_position.z,
                }, Color.red);
            }
        }
        return new BotTransformSyncRequestResponse();
    }

    BotExploseRequestResponse HandleBotExploseRequest(BotExploseRequest request_) {
        var bot = _botList.Find(bot_ => bot_.ID == request_.bot_id);
        if (bot != null) {
            _botList.Remove(bot);
            bot.Explose();
        }

        return new BotExploseRequestResponse();
    }

    TowerHpSyncRequestResponse HandleTowerHpSyncRequest(TowerHpSyncRequest request_) {
        var towerId = request_.tower_id;
        var normalized = request_.max_hp > 0 ? request_.hp / (float)request_.max_hp : 0;
        TaskInfo.TowerHp = normalized;
        switch (towerId) {
            case 0:
                break;
            case 1:
                break;
            default:
                break;
        }
        return new TowerHpSyncRequestResponse();
    }

    public SpawnBotRequestResponse HandleSpawnBotRequest(SpawnBotRequest request_) {
        switch (request_.bot_type) {
            case "spider_remote":
                {
                    var spiderRemoteObj = GameObject.Instantiate(SpiderRemotePrefab);
                    var spiderRemote = spiderRemoteObj.GetComponent<BotRemote>();
                    spiderRemote.ID = request_.bot_id;
                    spiderRemote.BotKilled += (bot_) => {
                        SockUtil.Level0BotKilled(new Level0BotKilledRequest(){
                            bot_id = (bot_ as BotRemote).ID
                        }, null);
                    };
                    _botList.Add(spiderRemote);

                    return new SpawnBotRequestResponse() {
                        errno = 0,
                    };
                }
            case "spider_king":
                {
                    var spiderKingObj = GameObject.Instantiate(SpiderKingPrefab);
                    // var spiderKing = spiderKingObj.GetComponent<Bot>();
                    var spiderKing = spiderKingObj.GetComponent<Actor>();
                    spiderKing.ID = request_.bot_id;
                    spiderKing.BotKilled += (bot_) => {
                        SockUtil.Level0BotKilled(new Level0BotKilledRequest(){
                            bot_id = bot_.ID
                        }, null);
                    };
                     _botList.Add(spiderKing as BotRemote);

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
}
