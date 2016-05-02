using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class OtherBotView : ViewBase {
    [SerializeField]
    private Vector3 _targetScale = new Vector3(0.002f, 0.002f, 0.002f);

    [SerializeField]
    private Slider _hpSlider;

    [SerializeField]
    private MainTankBot _bot;
    
    void Awake(){
        DataContext = _bot;
    }

    void Update(){
        var mainCamera = Camera.main.transform;
        transform.rotation = Quaternion.LookRotation(
                mainCamera.forward, Vector3.up);

        var localPosition = mainCamera.InverseTransformPoint(transform.position);
        var distance = localPosition.z;
        transform.localScale = distance * _targetScale;
    }

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null){
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
                case "MaxHP":
                case "HP":
                    {
                        // TODO: Copy from HPView.cs
                        var value = viewModel.HP / (float)viewModel.MaxHP;
                        if (_hpSlider.value != value)
                            _hpSlider.value = value;
                    }
                    break;
            }
        }
    }
}
