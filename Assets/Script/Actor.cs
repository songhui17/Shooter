using UnityEngine;
using System;
using System.Collections;

public class Actor : MonoBehaviour, INotifyPropertyChanged {

    #region Properties

    public int ID = 0;

    public event Action<Actor> BotKilled;
    protected void OnBotKilled() {
        if (BotKilled != null) {
            BotKilled(this);
        }
    }

    [SerializeField]
    private int _hp = 3;
    public int HP { 
        get { return _hp; }
        set {
            _hp = Mathf.Max(0, Mathf.Min(MaxHP, value));
            OnPropertyChanged("HP");
        }
    }

    [SerializeField]
    private int _maxHP = 3;
    public int MaxHP {
        get { return _maxHP; }
        set {
            _maxHP = Mathf.Max(1, value);
            OnPropertyChanged("MaxHP");
        }
    }

    public bool IsAlive { get { return HP > 0; } }

    #endregion

    #region INotifyPropertyChanged

    public event Action<INotifyPropertyChanged, object> PropertyChanged;
    protected void OnPropertyChanged(object property_) {
        if (PropertyChanged != null){
            PropertyChanged(this, property_);
        }
    }

    #endregion
}
