using UnityEngine;
using UnityEngine.UI;

public class TaskInfoView: ViewBase {
    [SerializeField]
    private Text _task1Text;

    [SerializeField]
    private Text _task2Text;

    [SerializeField]
    private Text _task3Text;

    void Awake() {
        LevelManager.Instance.PropertyChanged += HandlePropertyChanged;
        HandlePropertyChanged(LevelManager.Instance, "TaskInfo");
    }

    void OnDestroy() {
        LevelManager.Instance.PropertyChanged -= HandlePropertyChanged;
    }

    protected override void OnDataContextChanged(INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null) {
            HandlePropertyChanged(newContext_, "ActorLevelInfo");
            HandlePropertyChanged(newContext_, "Task1Info");
            HandlePropertyChanged(newContext_, "Task2Info");
            HandlePropertyChanged(newContext_, "Task3Info");
        }
    }

    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_) {
        var levelManager = sender_ as LevelManager;
        if (levelManager != null) {
            switch (property_ as string) {
                case "TaskInfo":
                    {
                        DataContext = levelManager.TaskInfo;
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        var viewModel = sender_ as TaskInfo;
        if (viewModel != null) {
            switch(property_ as string){
                case "ActorLevelInfo":
                    {
                        _task1Text.color = viewModel.ActorLevelInfo.star1 ? Color.green : Color.white; 
                        _task2Text.color = viewModel.ActorLevelInfo.star2 ? Color.green : Color.white; 
                        _task3Text.color = viewModel.ActorLevelInfo.star3 ? Color.green : Color.white;
                    }
                    break;
                case "Task1Info":
                    {
                        _task1Text.text = viewModel.Task1Info;
                    }
                    break;
                case "Task2Info":
                    {
                        _task2Text.text = viewModel.Task2Info;
                    }
                    break;
                case "Task3Info":
                    {
                        _task3Text.text = viewModel.Task3Info;
                    }
                    break;
                default:
                    break;
            }
        }
    }
    
}
