using UnityEngine;

public class ChapterInfoViewModel : ViewModelBase {
    private string _chapterTitle;
    public string ChapterTitle {
        get { return _chapterTitle ?? (_chapterTitle = "[STR] ChapterTitle"); }
        set { _chapterTitle = value; OnPropertyChanged("ChapterTitle"); }
    }

    public void StartFight(){
        // TODO: scene name
        var loadingViewModel = LoadingViewModel.Instance;
        if (loadingViewModel != null)
            loadingViewModel.StartFight("Prototype");
    }
}
