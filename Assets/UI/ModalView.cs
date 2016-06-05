using UnityEngine;
using UnityEngine.UI;

public class ModalView : ViewBase {
    [SerializeField]
    AnimatedActivate _messagePanel;

    [SerializeField]
    private Text _messageText;

    [SerializeField]
    private SpinningActivate _spinner;

    [SerializeField]
    private ModalViewModel _viewModel;
    void Awake() {
        DataContext = _viewModel;
    }

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);
        if (newContext_ != null){
            HandlePropertyChanged(newContext_, "Show");
            HandlePropertyChanged(newContext_, "Message");
            HandlePropertyChanged(newContext_, "ShowSpinner");
        }else{
            _messageText.text = "";
        }
    }

    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
        var viewModel = sender_ as ModalViewModel;
        if (viewModel != null){
            switch (property_ as string){
                case "Show":
                    {
                        if (viewModel.Show && viewModel.AutoHide) {
                            _messagePanel.SetActiveAnimated(true, () => {
                                _messagePanel.SetActiveAnimated(false, null);
                            }, 0, true);
                        }else{
                            _messagePanel.SetActive(viewModel.Show);
                        }
                    }
                    break;
                case "Message":
                    {
                        _messageText.text = viewModel.Message;
                    }
                    break;
                case "ShowSpinner":
                    {
                        _spinner.SetActive(viewModel.ShowSpinner);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
