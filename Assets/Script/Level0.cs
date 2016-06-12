using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Shooter;
using Vector3 = UnityEngine.Vector3;

public class Level0 : MonoBehaviour, ILevel {
    public TaskInfo TaskInfo = new TaskInfo();
    public TaskInfo GetTaskInfo() {
        return TaskInfo;
    }

    private GameObject _spiderPrefab;
    private GameObject SpiderPrefab {
        get {
            return _spiderPrefab ?? (_spiderPrefab = Resources.Load<GameObject>("Spider"));
        }
    }

    void Awake() {
        TaskInfo.LevelInfo = LevelManager.Instance.CurrentLevelInfo;
        SockUtil.Instance.RegisterHandler<ActorLevelInfoSyncRequest, ActorLevelInfoSyncRequestResponse>("actor_level_info_sync", HandleActorLevelInfoSyncRequest, force_:true);
    }

    ActorLevelInfoSyncRequestResponse HandleActorLevelInfoSyncRequest(ActorLevelInfoSyncRequest request_) {
        Debug.Log(request_);
        TaskInfo.ActorLevelInfo = request_.actor_level_info;
        return new ActorLevelInfoSyncRequestResponse();
    }

    public SpawnBotRequestResponse HandleSpawnBotRequest(SpawnBotRequest request_) {
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
                        SpiderPrefab, position, rotation) as GameObject;
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
}
