using UnityEngine;
using System;
using System.Collections.Generic;

using Shooter;

public class ChapterInfoViewModel : ViewModelBase {
    #region Properties

    private Chapter _chapter;
    public Chapter Chapter {
        get { return _chapter ?? (_chapter = new Chapter()); }
        set { _chapter = value; OnPropertyChanged("Chapter"); }
    } 

    private Level _level;
    public Level Level {
        get {
            if (_level == null){
                if (Chapter.LevelList.Count > 1){
                    _level = Chapter.LevelList[0];
                    return _level;
                }else{
                    // throw new Exception("Chapter is not initialized" +
                            // " or not Levels in this chapter");
                    return new Level();
                }
            }else{
                return _level;
            }
        }
        set { _level = value; OnPropertyChanged("Level"); }
    }

    private string _chapterTitle;
    public string ChapterTitle {
        get { return _chapterTitle ?? (_chapterTitle = "[STR] ChapterTitle"); }
        set { _chapterTitle = value; OnPropertyChanged("ChapterTitle"); }
    }

    #endregion 

    public void StartFight(){
        var loadingViewModel = LoadingViewModel.Instance;
        if (loadingViewModel != null)
            loadingViewModel.StartFight(Level);

        SockUtil.Instance.SendRequest<StartLevelRequest, StartLevelRequestResponse>(
            "start_level", new StartLevelRequest() {
                actor_id = 0,
                level_id = 0,
            }, null);
    }

    void Awake(){
        Chapter = new Chapter(){
            ID = "二",
            Title = "巨人的废墟",
            LevelList = new List<Level>(){
                new Level(){
                    ID = "1",
                    Title = "虎口脱险",
                    Description = "躲避泰坦袭击",
                    TaskDescriptionList = {
                        "击退泰坦",
                        "找到地道",
                        "杀死1个僵尸",
                    },
                    Scene = "Prototype",
                },
                new Level(){
                    ID = "2",
                    Title = "一触即发",
                    Description = "搜寻科学家“影”博士",
                    TaskDescriptionList = {
                        "摧毁虫穴",
                        "50％以上血量通关",
                        "使用手雷杀死5个敌人",
                    },
                    Scene = "Prototype",
                },
            },
        };
        foreach (var level_ in Chapter.LevelList){
            level_.Chapter = Chapter;
        }
        Level = Chapter.LevelList[0];
    }
}
