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
    }

    public void SetActorLeveInfo(List<ActorLevelInfo> leveInfo_) {
        ActorLevelInfoList = leveInfo_;
    }
    public void SetLevelInfo(List<LevelInfo> leveInfo_) {
        LevelInfoList = leveInfo_;
    }
}
