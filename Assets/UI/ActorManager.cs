using UnityEngine;
using System;
using Shooter;

public enum ENUM_ACTOR_STATE {

}

public class ActorManager : MonoBehaviour {
    private static ActorManager _instance;
    public static ActorManager Instance { get { return _instance; } }

    [SerializeField]
    private CreateActorViewModel _createActorViewModel;

    private bool _hasActor { get { return _actorId != -1; } }
    private int _actorId = -1;

    private int _requestCount = 0;
    public int RequestCount {
        get { return _requestCount; }
        set {
            _requestCount = value;
            if (_requestCount == 0) {
                ModalViewModel.Instance.Hide();
            }else{
                ModalViewModel.Instance.ShowMessage("Sending Request...");
            }
        }
    }

    void Awake() {
        if (_instance != null){
            enabled = false;
            Destroy(gameObject);
            Debug.LogWarning("Multiple instance of ActorManager");
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        SockUtil.Instance.SendRequest<GetAccountInfoRequest, GetAccountInfoRequestResponse>(
            "get_account_info", new GetAccountInfoRequest() {
                username = LoginViewModel.Instance.Account
            }, OnGetAccountInfo);
    }

    void OnGetAccountInfo(GetAccountInfoRequestResponse response_, int requestId_) {
        Debug.Log(response_);
        var errno = response_.errno;
        if (errno == 0) {
            _createActorViewModel.ShowPanel = false;
            _actorId = response_.account_info.actor_id;
            _createActorViewModel.ShowPanel = !_hasActor;

            if (_hasActor) {
                GetActorInfo();
                GetActorLevelInfo();
                GetLevelInfo();
            }
        }else if(errno == 3){
            // _hasActor = false;
            // _createActorViewModel.ShowPanel = true;
            throw new NotImplementedException("Login required.");
        }
    }

    public void CreateActor(int typeId_) {
        string[] actorTypes = {
            "jugg", "sf", "pa",
        };
        SockUtil.Instance.SendRequest<CreateActorRequest, CreateActorRequestResponse>(
                "create_actor", new CreateActorRequest() { 
                    username = LoginViewModel.Instance.Account,
                    actor_type = actorTypes[typeId_]
                }, OnCreateActor);
    }

    void OnCreateActor(CreateActorRequestResponse response_, int requestId_) {
        Debug.Log(response_);
        var errno = response_.errno;
        if (errno == 0) {
            _createActorViewModel.ShowPanel = false;
            GetActorInfo();
            GetActorLevelInfo();
            GetLevelInfo();
        }else if(errno == 3) {
            // TODO: login required
        }
    }

    void GetActorInfo() {
        RequestCount++;
        SockUtil.Instance.SendRequest<GetActorInfoRequest, GetActorInfoRequestResponse>(
                "get_actor_info", new GetActorInfoRequest() {
                    username = LoginViewModel.Instance.Account,
                }, OnGetActorInfo);
    }

    void OnGetActorInfo(GetActorInfoRequestResponse response_, int requestId_) {
        Debug.Log(response_);
        RequestCount--;
    }

    void GetActorLevelInfo() {
        RequestCount++;
        SockUtil.Instance.SendRequest<GetActorLevelInfoRequest, GetActorLevelInfoRequestResponse>(
                "get_actor_level_info", new GetActorLevelInfoRequest() {
                    username = LoginViewModel.Instance.Account,
                }, OnGetActorLevelInfo);
    }
    
    void OnGetActorLevelInfo(GetActorLevelInfoRequestResponse response_, int requestId_) {
        RequestCount--;
        var info = "OnGetActorLevelInfo:\n";
        foreach (var levelInfo in response_.actor_level_info)
        {
            info += levelInfo + "\n";
        }
        Debug.Log(info);
        LevelManager.Instance.SetActorLeveInfo(response_.actor_level_info);

        if (response_.errno == 0){
        }else{
            throw new NotImplementedException("Handle OnGetActorLevelInfo error");
        }
    }

    void GetLevelInfo() {
        RequestCount++;
        SockUtil.Instance.SendRequest<GetLevelInfoRequest, GetLevelInfoRequestResponse>(
                "get_level_info", new GetLevelInfoRequest() {
                    username = LoginViewModel.Instance.Account,
                }, OnGetLevelInfo);
    }

    void OnGetLevelInfo(GetLevelInfoRequestResponse response_, int requestId_) {
        RequestCount--;
        var info = "OnGetLevelInfo:\n";
        foreach (var levelInfo in response_.level_info)
        {
            info += levelInfo + "\n";
        }
        Debug.Log(info);
        LevelManager.Instance.SetLevelInfo(response_.level_info);
        if (response_.errno == 0){
        }else{
            throw new NotImplementedException("Handle OnGetLevelInfo error");
        }
    }
}
