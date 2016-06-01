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
}
