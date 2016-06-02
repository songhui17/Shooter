using UnityEngine;
using System.Collections.Generic;

public class LevelManagerView : ViewBase {
    [SerializeField]
    private List<LevelView> _levelViewList;

    void Start() {
        DataContext = LevelManager.Instance;
    }

    [ContextMenu("Get LevelView in Children")]
    void GetLevelView() {
        _levelViewList = new List<LevelView>();
        _levelViewList.AddRange(GetComponentsInChildren<LevelView>());
    }


    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null){
            HandlePropertyChanged(newContext_, "ActorLevelInfoList");
            HandlePropertyChanged(newContext_, "LeveInfoList");
        }else{
        }
    }
    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
        var viewModel = sender_ as LevelManager;
        switch (property_ as string){
            case "ActorLevelInfoList":
                {
                    var gotNotPassed = false;
                    for (int i = 0; i < _levelViewList.Count; i++) {
                        var levelViewModel = _levelViewList[i].DataContext as LevelViewModel;
                        var levelInfo = viewModel.ActorLevelInfoList.Find(item_ => item_.level_id == i);
                        levelViewModel.ActorLevelInfo = levelInfo;
                        if (!gotNotPassed) {
                            if (!levelViewModel.CanFight)
                                levelViewModel.CanFight = true;

                            if (levelInfo == null || !levelInfo.passed) {
                                gotNotPassed = true;
                            }
                        }else{
                            if (levelViewModel.CanFight)
                                levelViewModel.CanFight = false;
                        }
                    }
                }
                break;
            case "LevelInfoList":
                {
                    for (int i = 0; i < _levelViewList.Count; i++) {
                        var levelViewModel = _levelViewList[i].DataContext as LevelViewModel;
                        var levelInfo = viewModel.LevelInfoList.Find(item_ => item_.level_id == i);
                        levelViewModel.LevelInfo = levelInfo;
                    }
                }
                break;
            default:
                break;
        }
    }
}
