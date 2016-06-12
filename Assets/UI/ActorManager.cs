using UnityEngine;
using System;
using Shooter;

using ActorInfo = Shooter.Actor;

public enum ENUM_ACTOR_STATE {

}

public class ActorManager : ViewModelBase {
    private static ActorManager _instance;
    public static ActorManager Instance { get { return _instance; } }

    [SerializeField]
    private CreateActorViewModel _createActorViewModel;

    // TODO
    // [SerializeField]
    // private GameObject _actorShow;


    private ActorInfo _actorInfo;
    public ActorInfo ActorInfo {
        get { return _actorInfo; }
        private set { _actorInfo = value; OnPropertyChanged("ActorInfo");}
    }
    private bool _hasActor { get { return ActorInfo != null; } }

    //TODO:!!!!
    public event Action<int> MaxAmmoChanged;
    public int MaxAmmo {
        get { return ActorInfo.max_ammo; }
        set {
            ActorInfo.max_ammo = value;
            if (MaxAmmoChanged != null) {
                MaxAmmoChanged(value);
            }
        }
    }

    private int _requestCount = 0;
    public int RequestCount {
        get { return _requestCount; }
        set {
            _requestCount = value;
            if (_requestCount == 0) {
                ModalViewModel.Instance.ShowSpinner = false;
                // ModalViewModel.Instance.Hide();
            }else{
                ModalViewModel.Instance.ShowSpinner = true;
                // ModalViewModel.Instance.ShowMessage("Sending Request...");
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

        RequestCount++;
        var request = new GetAccountInfoRequest() {
            username = LoginManager.Instance.Account
        };
        SockUtil.GetAccountInfo(request, OnGetAccountInfo);

        LoadingViewModel.Instance.Loaded += level_ => {
            if (level_ == "Lobby") {
                if (_hasActor) {
                    GetActorInfo();
                    GetActorLevelInfo();
                }
            }
        };
    }

    void OnGetAccountInfo(GetAccountInfoRequestResponse response_, int requestId_) {
        Debug.Log(response_);
        RequestCount--;
        var errno = response_.errno;
        if (errno == (int)ENUM_SHOOTER_ERROR.E_OK) {
            _createActorViewModel.ShowPanel = false;
            var actorId = response_.account_info.actor_id;
            _createActorViewModel.ShowPanel = actorId == -1;

            if (actorId != -1) {
                GetActorInfo();
                GetActorLevelInfo();
                GetLevelInfo();
            }
        } else {
            ModalViewModel.Instance.ShowMessage(StringTable.Value("Actor.GetAccountInfo.Failed"));
        }
        // else if(errno == 3){
        //     // _hasActor = false;
        //     // _createActorViewModel.ShowPanel = true;
        //     // throw new NotImplementedException("Login required.");
        // }
    }

    public void CreateActor(int typeId_) {
        string[] actorTypes = {
            "jugg", "sf", "pa",
        };

        var request = new CreateActorRequest() {
            username = LoginManager.Instance.Account,
            actor_type = actorTypes[typeId_],
        };
        SockUtil.CreateActor(request, OnCreateActor);
    }

    void OnCreateActor(CreateActorRequestResponse response_, int requestId_) {
        Debug.Log(response_);
        var errno = response_.errno;
        if (errno == (int)ENUM_SHOOTER_ERROR.E_OK) {
            _createActorViewModel.ShowPanel = false;
            GetActorInfo();
            GetActorLevelInfo();
            GetLevelInfo();
        } else {
            // throw new NotImplementedException("");
            // TODO: drop?
            ModalViewModel.Instance.ShowMessage(StringTable.Value("Actor.CreateActor.Failed"));
        }
    }

    void GetActorInfo() {
        RequestCount++;

        var request = new GetActorInfoRequest() {
            username = LoginManager.Instance.Account,
        };
        SockUtil.GetActorInfo(request, OnGetActorInfo);
    }

    void OnGetActorInfo(GetActorInfoRequestResponse response_, int requestId_) {
        Debug.Log(response_);
        RequestCount--;
        if (response_.errno == (int)ENUM_SHOOTER_ERROR.E_OK) {
            ActorInfo = response_.actor_info;
            // TODO:
            // _actorShow.SetActive(true);
        } else {
            // throw new NotImplementedException("");
            // TODO: drop?
            ModalViewModel.Instance.ShowMessage(StringTable.Value("Actor.GetActorInfo.Failed"));
        }
    }

    public void GetActorLevelInfo() {
        RequestCount++;
        var request = new GetActorLevelInfoRequest() {
            username = LoginManager.Instance.Account,
        };
        SockUtil.GetActorLevelInfo(request, OnGetActorLevelInfo);
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

        if (response_.errno == (int)ENUM_SHOOTER_ERROR.E_OK){
        }else{
            // throw new NotImplementedException("Handle OnGetActorLevelInfo error");
            // TODO: drop
            ModalViewModel.Instance.ShowMessage(StringTable.Value("Actor.GetActorLevelInfo.Failed"));
        }
    }

    void GetLevelInfo() {
        RequestCount++;
        var request = new GetLevelInfoRequest() {
            username = LoginManager.Instance.Account,
        };
        SockUtil.GetLevelInfo(request, OnGetLevelInfo);
    }

    void OnGetLevelInfo(GetLevelInfoRequestResponse response_, int requestId_) {
        RequestCount--;
        var info = "OnGetLevelInfo:\n";
        foreach (var levelInfo in response_.level_info)
        {
            info += levelInfo + "\n";
        }
        Debug.Log(info);
        if (response_.errno == (int)ENUM_SHOOTER_ERROR.E_OK){
            LevelManager.Instance.SetLevelInfo(response_.level_info);
        }else{
            // throw new NotImplementedException("Handle OnGetLevelInfo error");
            ModalViewModel.Instance.ShowMessage(StringTable.Value("Level.GetLevelInfo.Failed"));
        }
    }
}
