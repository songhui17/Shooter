using UnityEngine;
using UnityEngine.UI;

public class BotHPView : ViewBase {
    [SerializeField]
    private Slider _hpSlider;

    protected virtual void OnDataContextChanged(INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null) {
            HandlePropertyChanged(newContext_, "HP");
            HandlePropertyChanged(newContext_, "name");
        }
    }

    protected virtual void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_) {
        var viewModel = sender_ as Bot;
        if (viewModel != null) {
            switch(property_ as string){
                case "HP":
                    {
                        // TODO: Copy from HPView.cs
                        var value = viewModel.HP / (float)viewModel.MaxHP;
                        if (_hpSlider.value != value)
                            _hpSlider.value = value;
                    }
                    break;
                case "name":
                    break;
            }
        }
    }
}
