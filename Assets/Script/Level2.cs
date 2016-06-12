using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Shooter;
using Vector3 = UnityEngine.Vector3;

public class Level2 : MonoBehaviour {
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

    private Transform _playerTransform;

    void Awake() {
        SockUtil.Instance.RegisterHandler<BotTransformSyncRequest, BotTransformSyncRequestResponse>("bot_transform_sync", HandleBotTransformSyncRequest, force_:true);
        SockUtil.Instance.RegisterHandler<BotExploseRequest, BotExploseRequestResponse>("bot_explose", HandleBotExploseRequest, force_:true);
        SockUtil.Instance.RegisterHandler<TowerHpSyncRequest, TowerHpSyncRequestResponse>("tower_hp_sync", HandleTowerHpSyncRequest, force_:true);
        SockUtil.Instance.RegisterHandler<SpawnItemRequest, SpawnItemRequestResponse>("spawn_item", HandleSpawnItemRequest, force_:true);
        SockUtil.Instance.RegisterHandler<UseItemRequest, UseItemRequestResponse>("use_item", HandleUseItemRequest, force_:true);

        SockUtil.Instance.RegisterHandler<BotPlayAnimationRequest, BotPlayAnimationRequestResponse>("bot_play_animation", HandleBotPlayAnimationRequest, force_:true);
    }

    IEnumerator Start() {
        var wait = new WaitForSeconds(1);
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
                    var spiderKing = spiderKingObj.GetComponent<Bot>();
                    spiderKing.ID = request_.bot_id;
                    spiderKing.BotKilled += (bot_) => {
                        SockUtil.Level0BotKilled(new Level0BotKilledRequest(){
                            bot_id = (bot_ as Bot).ID
                        }, null);
                    };
                    // _botList.Add(spiderKing);

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
