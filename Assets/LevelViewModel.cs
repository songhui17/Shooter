using UnityEngine;
using System.Collections.Generic;

using Shooter;

public class LevelViewModel : ViewModelBase {
    private ActorLevelInfo _actorLevelInfo;
    public ActorLevelInfo ActorLevelInfo
    {
        get { return _actorLevelInfo?? (_actorLevelInfo= new ActorLevelInfo());}
        set { _actorLevelInfo= value; OnPropertyChanged("ActorLevelInfo"); }
    }

    private LevelInfo _levelInfo;
    public LevelInfo LevelInfo
    {
        get { return _levelInfo?? (_levelInfo= new LevelInfo());}
        set { _levelInfo= value; OnPropertyChanged("LevelInfo"); }
    }

    private bool _canFight;
    public bool CanFight
    {
        get { return _canFight; }
        set { _canFight = value; OnPropertyChanged("CanFight"); }
    }

    private bool _latest;
    public bool Latest
    {
        get { return _latest; }
        set { _latest = value; OnPropertyChanged("Latest"); }
    }

    public void StartLevel() {
        var levelId = LevelInfo.level_id;
        var actorId = ActorManager.Instance.ActorInfo.actor_id;
        var request = new StartLevelRequest() {
            actor_id = actorId,
            level_id = levelId,
        };
        Debug.Log("StartLevel request:\n" + request);
        SockUtil.StartLevel(request, OnStartLevel);
    }

    void OnStartLevel(StartLevelRequestResponse response_, int requestId_) {
        Debug.Log(response_);

        switch ((ENUM_SHOOTER_ERROR)response_.errno) {
            case ENUM_SHOOTER_ERROR.E_OK:
                break;
            default:
                ModalViewModel.Instance.ShowMessage(
                    StringTable.Value("Level.StartLevel.Failed.NoSuchLevel"), true);
                break;
        }
    }
}
