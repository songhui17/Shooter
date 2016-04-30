using UnityEngine;

public class ChapterInfoViewModel : ViewModelBase {
    [SerializeField]
    private LoadingViewModel _loadingViewModel;

    private string _chapterTitle;
    public string ChapterTitle {
        get { return _chapterTitle ?? (_chapterTitle = "[STR] ChapterTitle"); }
        set { _chapterTitle = value; OnPropertyChanged("ChapterTitle"); }
    }

    public void StartFight(){
        // TODO: scene name
        _loadingViewModel.StartFight("Prototype");
    }
}
