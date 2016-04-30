using UnityEngine;
using System;

public class DataModelBase : MonoBehaviour, INotifyPropertyChanged {
    public event Action<INotifyPropertyChanged, object> PropertyChanged;
    protected void OnPropertyChanged(object property_) {
        if (PropertyChanged != null){
            PropertyChanged(this, property_);
        }
    }
}
