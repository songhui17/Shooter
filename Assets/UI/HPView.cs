using UnityEngine;
using UnityEngine.UI;

public class HPView : ViewBase {
    [SerializeField]
    private Slider _hpSlider;

    [SerializeField]
    private Text _hpText;

    [SerializeField]
    private Bot _bot;
    
    void Awake(){
        DataContext = _bot;
    }

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null){
            // HandlePropertyChanged(newContext_, "MaxHP");
            _hpSlider.minValue = 0;
            _hpSlider.maxValue = 1;
            HandlePropertyChanged(newContext_, "HP");
        }
    }

    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
        var viewModel = sender_ as Actor;
        if (viewModel != null){
            switch (property_ as string){
                case "HP":
                case "MaxHP":
                    {
                        var value = viewModel.HP / (float)viewModel.MaxHP;
                        if (_hpSlider.value != value)
                            _hpSlider.value = value;

                        var hpString = viewModel.HP.ToString();
                        if (_hpText != null && _hpText.text != hpString)
                            _hpText.text = hpString;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
