using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ChapterInfoView : ViewBase {

    #region Fields
    [SerializeField]
    private bool _verbose;

    [SerializeField]
    private Text _chapterTitleText;
    [SerializeField]
    private Text _levelTitleText;
    [SerializeField]
    private Text _levelDescriptionText;
    [SerializeField]
    private List<Text> _taskListText;
    [SerializeField]
    private List<Button> _taskButtonList;
    #endregion

    void Awake(){
        DataContext = GetComponent<ChapterInfoViewModel>();

        var info = string.Format("_taskButtonList: {0}\n", _taskButtonList.Count);
        for (int i = 0; i < _taskButtonList.Count; i++){
            var index = i;
            _taskButtonList[i].onClick.AddListener(() => {
                if (_verbose)
                    Debug.Log(string.Format("onClick index: {0}", index));
                OnTaskButton(index);
            });
        }
        if (_verbose) Debug.Log(info);
    }

    void OnTaskButton(int index_){
        var viewModel = DataContext as ChapterInfoViewModel;
        if (viewModel == null){
            Debug.LogError("viewModel is null: not initialized properly");
            return;
        }

        var chapter = viewModel.Chapter;
        if (_verbose){
            var info = string.Format(
                "index_: {0}, chapter.LevelList.Count: {1}",
                index_, chapter.LevelList.Count);
            Debug.Log(info);
        }

        if (index_ < chapter.LevelList.Count){
            viewModel.Level = chapter.LevelList[index_];
        }
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
            // TODO: order is important
            HandlePropertyChanged(newContext_, "Chapter");
            HandlePropertyChanged(newContext_, "Level");
        }else{
            // TODO: reset UI
        }
    }

    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
        if (HandleViewModelPropertyChanged(sender_ as ChapterInfoViewModel, property_)) return;
        if (HandleChapterPropertyChanged(sender_ as Chapter, property_)) return;
        if (HandleLevelPropertyChanged(sender_ as Level, property_)) return;
    }

    private bool HandleLevelPropertyChanged(
            Level level_, object property_){
        if (level_ != null){
            switch (property_ as string){
                case "ID":
                case "Title":
                    {
                        var viewModel = DataContext as ChapterInfoViewModel;
                        // NOTE: Bind only if must to
                        var chapterId = viewModel.Chapter.ID;
                        if (_levelTitleText != null){
                            var format = "{0}-{1} {2}";
                            var title = string.Format(
                               format, chapterId, level_.ID, level_.Title);
                            Debug.Log(title);

                            _levelTitleText.text = title;
                        }
                    }
                    break;
                case "Description":
                    {
                        var desription = level_.Description;
                        if (_levelDescriptionText != null){
                            _levelDescriptionText.text = desription;
                        }
                    }
                    break;
                case "TaskDescriptionList":
                    {
                        var taskList = level_.TaskDescriptionList;
                        for (int i = 0; i < 3; i++){
                            if (i < taskList.Count)
                                _taskListText[i].text = "[x] " + taskList[i];
                            else
                                _taskListText[i].text = "[x] 任务" + i;
                        }
                    }
                    break;
                default:
                    break;
            }
            return true;
        }else{
            return false;
        }
    }
        
    private bool HandleChapterPropertyChanged(
            Chapter chapter_, object property_){
        if (chapter_ != null){
            switch (property_ as string){
                case "ID":
                case "Title":
                    {
                        if (_chapterTitleText != null){
                            var id = chapter_.ID;
                            var title = chapter_.Title;
                            if (title != _chapterTitleText.text){
                                var format = "第{0}章 {1}";
                                _chapterTitleText.text =
                                    string.Format(format, id, title);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return true;
        }else{
            return false;
        }
    }

    private bool HandleViewModelPropertyChanged(
            ChapterInfoViewModel viewModel_, object property_){
        if (viewModel_ != null){
            switch (property_ as string){
                case "Chapter":
                    {
                        var chapter = viewModel_.Chapter;
                        Bind("Chapter", chapter);
                        HandleChapterPropertyChanged(chapter, "Title");
                    }
                    break;
                case "Level":
                    {
                        var level = viewModel_.Level;
                        Bind("Level", level);
                        // HandleLevelPropertyChanged(level, "ID");
                        HandleLevelPropertyChanged(level, "Title");
                        HandleLevelPropertyChanged(level, "Description");
                        HandleLevelPropertyChanged(level, "TaskDescriptionList");
                    }
                    break;
                default:
                    break;
            }
            return true;
        }else{
            return false;
        }
    }
}
