using UnityEngine;
using UnityEngine.UI;

public class ChapterInfoView : ViewBase {

#region Fields
    // [SerializeField]
    // private GameObject _loadingScreen;
#endregion

    void Awake(){
        // DataContext = gameObject.AddComponent<ChapterInfoViewModel>();
        DataContext = GetComponent<ChapterInfoViewModel>();
    }

    public void OnStartFightButton() {
        var viewModel = DataContext as ChapterInfoViewModel;
        if (viewModel != null){
            viewModel.StartFight();
        }
    }

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null){
        }else{
            // TODO: reset UI
        }
    }
}
