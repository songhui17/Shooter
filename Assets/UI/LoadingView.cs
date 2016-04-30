using UnityEngine;
using UnityEngine.UI;

public class LoadingView : ViewBase {
#region Fields
    [SerializeField]
    private bool _verbose = true;

    [SerializeField]
    private Slider _progressSlider;
#endregion

    private static LoadingView _instance;
    public static LoadingView Instance {
        get { return _instance; }
        private set { _instance = value; }
    }

    void Awake(){
        if (Instance != null){
            enabled = false;
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
        DataContext = GetComponent<LoadingViewModel>();
    }

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null){
            HandlePropertyChanged(newContext_, "IsLoading");
            HandlePropertyChanged(newContext_, "LoadingProgress");
        }else{
        }
    }

    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
        var viewModel = sender_ as LoadingViewModel;
        if (viewModel != null){
            switch (property_ as string){
                case "IsLoading":
                    {
                        var isLoading = viewModel.IsLoading;
                        if (_verbose){
                            Debug.Log(string.Format(
                                "isLoading: {0}", isLoading));
                        }
                        gameObject.SetActive(isLoading);
                    }
                    break;
                case "LoadingProgress":
                    {
                        if (_progressSlider != null){
                            var loadingProgress = viewModel.LoadingProgress;
                            if (loadingProgress != _progressSlider.value){
                                _progressSlider.value = loadingProgress;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

}
