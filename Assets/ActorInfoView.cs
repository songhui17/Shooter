using UnityEngine;
using UnityEngine.UI;


public class ActorInfoView : ViewBase {

    [SerializeField]
    private Text _levelText;
    [SerializeField]
    private Text _goldText;
    [SerializeField]
    private Text _expText;

    void Start() {
        Bind("ActorManager", ActorManager.Instance);
    }

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_) {
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null) {
            // TODO: Actor -> DataModelBase?
        }else{
            //TODO
        }
    }
    
    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
        var actorManager = sender_ as ActorManager;
        if (actorManager != null) {
            var actorInfo = actorManager.ActorInfo;
            if (actorInfo != null) {
                _levelText.text = actorInfo.level.ToString();//TODO
                _goldText.text = actorInfo.gold.ToString();//TODO
                _expText.text = actorInfo.experience.ToString(); //TODO
            }
        }
    }
}
