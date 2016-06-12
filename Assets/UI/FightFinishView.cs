using UnityEngine;
using UnityEngine.UI;

public class FightFinishView : ViewBase {
    #region Fields
    [SerializeField]
    private bool _verbose = true;

    [SerializeField]
    private GameObject _winPanel;

    [SerializeField]
    private GameObject _losePanel;
    #endregion

    void Awake(){
        DataContext = GetComponent<FightFinishViewModel>();
    }

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);
        if (newContext_ != null){
            HandlePropertyChanged(newContext_, "IsFightFinished");
        }else{
        }
    }

    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
        var viewModel = sender_ as FightFinishViewModel;
        if (viewModel != null){
            switch (property_ as string){
                case "IsFightFinished":
                    {
                        if (viewModel.Win) {
                            _winPanel.SetActive(true);
                            _losePanel.SetActive(false);
                        }else{
                            _winPanel.SetActive(false);
                            _losePanel.SetActive(true);
                        }
                        var isFightFinished = viewModel.IsFightFinished;
                        if (_verbose)
                            Debug.Log(string.Format(
                                    "isFightFinished: {0}", isFightFinished));
                        gameObject.SetActive(isFightFinished);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void OnBackToLobbyButton(){
        var viewModel = DataContext as FightFinishViewModel;
        if (viewModel != null){
            viewModel.BackToLobby();
        }
    }
}
