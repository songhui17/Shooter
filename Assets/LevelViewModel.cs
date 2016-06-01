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

    private bool _canFight;
    public bool CanFight
    {
        get { return _canFight; }
        set { _canFight = value; OnPropertyChanged("CanFight"); }
    }
}
