using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/*

## StartFight: Level

View:
(1) Group: Level title, Level snapshot, Level tasks
(2) Group: Loading progress, Tips

ViewModel: Scene to load, Prototype, etc.

## BackToLobby: 

View:
(1) Group:
    Loading snapshot;
    Loading Title, e.g. Lobby

(2) Group: Loading progress, Tips

ViewModel: Scene to load, e.g. Lobby

**/
public class LoadingView : ViewBase {
    #region Fields

    [SerializeField]
    private bool _verbose = true;

    [SerializeField]
    private Text _tipsText;

    [SerializeField]
    private Slider _progressSlider;

    [SerializeField]
    private GameObject _backToLobbyPanel;
    [SerializeField]
    private Text _backToLobbyTitleText;

    [SerializeField]
    private GameObject _startFightPanel;
    [SerializeField]
    private Text _startFightTitleText;

    [SerializeField]
    private List<Text> _startFightTaskListText;
    
    #endregion

    #region Properties

    private static LoadingView _instance;
    public static LoadingView Instance {
        get { return _instance; }
        private set { _instance = value; }
    }

    #endregion

    #region MonoBehaviour

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

    #endregion

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null){
            HandlePropertyChanged(newContext_, "IsLoading");
            HandlePropertyChanged(newContext_, "LoadingProgress");
            HandlePropertyChanged(newContext_, "Tips");
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

                        if (isLoading){
                            switch (viewModel.LoadingType){
                                case LoadingType.StartFight:
                                    {
                                        StartFightView(viewModel);
                                    }
                                    break;
                                case LoadingType.BackToLobby:
                                    {
                                        BackToLobbyView();
                                    }
                                    break;
                                default:
                                    break;
                            }    
                        }
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
                case "Tips":
                    {
                        _tipsText.text = viewModel.Tips;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void StartFightView(LoadingViewModel viewModel_){
        _startFightPanel.SetActive(true);
        _backToLobbyPanel.SetActive(false);

        var level = viewModel_.CurrentLevel;
        var title = string.Format(
                "第{0}关 {1}", level.ID, level.Title);
        _startFightTitleText.text = title; 

        // TODO: copy from ChapterInfoView.cs
        var taskList = level.TaskDescriptionList;
        for (int i = 0; i < 3; i++){
            if (i < taskList.Count)
                _startFightTaskListText[i].text = taskList[i];
            else
                _startFightTaskListText[i].text = "任务" + i;
        }
    }

    private void BackToLobbyView(){
        _startFightPanel.SetActive(false);
        _backToLobbyPanel.SetActive(true);

        _backToLobbyTitleText.text = "游戏大厅";
    }
}
