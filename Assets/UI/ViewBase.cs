using UnityEngine;
using System;
using System.Collections.Generic;

public interface INotifyPropertyChanged {
    event Action<INotifyPropertyChanged, object> PropertyChanged;
}

public class ViewBase : MonoBehaviour {
    private Dictionary<string, INotifyPropertyChanged> _dataContextDict =
        new Dictionary<string, INotifyPropertyChanged>();

    private INotifyPropertyChanged _dataContext;
    public INotifyPropertyChanged DataContext{
        get { return _dataContext; }
        set {
            OnDataContextChanged(value);
            _dataContext = value;
        }
    }
    
    protected virtual void OnDataContextChanged(INotifyPropertyChanged newContext_){
        if (_dataContext != null)
            _dataContext.PropertyChanged -= HandlePropertyChanged;

        if (newContext_ != null){
            _dataContext = newContext_;
            _dataContext.PropertyChanged += HandlePropertyChanged;
        }

        foreach(var key in _dataContextDict.Keys){
            _dataContextDict[key].PropertyChanged -= HandlePropertyChanged;
        }
        _dataContextDict.Clear();
    }

    protected void Bind(string propertyPath_, INotifyPropertyChanged context_){
        if (propertyPath_ == null) return;

        if (_dataContextDict.ContainsKey(propertyPath_)){
            _dataContextDict[propertyPath_].PropertyChanged -= HandlePropertyChanged;
        }

        _dataContextDict[propertyPath_] = context_;
        context_.PropertyChanged += HandlePropertyChanged;
    }

    protected virtual void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
    }

    void OnDestroy() {
        DataContext = null;
    }
}
