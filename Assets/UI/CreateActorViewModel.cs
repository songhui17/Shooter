using UnityEngine;

public class CreateActorViewModel : ViewModelBase {
    private bool _showPanel;
    public bool ShowPanel {
        get { return _showPanel; }
        set { _showPanel = value; OnPropertyChanged("ShowPanel"); }
    }

    private int _actorTypeID = 0;
    public int ActorTypeID {
        get { return _actorTypeID; }
        set { _actorTypeID = value; OnPropertyChanged("ActorTypeID"); }
    }

    public void CreateActor() {
        ActorManager.Instance.CreateActor(_actorTypeID);
    }
}
