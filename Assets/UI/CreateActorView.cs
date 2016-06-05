using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateActorView : ViewBase {
    [SerializeField]
    private GameObject _createActorPanel;

    [SerializeField]
    private CreateActorViewModel _viewModel;

    [SerializeField]
    private List<Button> _actorButtonList;

    [SerializeField]
    private List<Text> _actorTextList;

    [SerializeField]
    Text _actorNameText;

    [SerializeField]
    List<string> _actorNameList;

    void Awake() {
        DataContext = _viewModel;

        for (int id = 0; id < _actorButtonList.Count; id++) {
            var button_ = _actorButtonList[id];
            var index = id;
            button_.onClick.AddListener(() =>
            {
                if (index > 0) {
                    ModalViewModel.Instance.ShowMessage(
                        string.Format(StringTable.Value("Actor.CreateActor.Failed.NotImplemented"),
                                      _actorNameList[index]), true);
                }else {
                    var viewModel = DataContext as CreateActorViewModel;
                    var actorTypeId = viewModel.ActorTypeID;
                    if (index != actorTypeId) {
                        viewModel.ActorTypeID = index;
                    }
                }
            });
        }
    }

    public void OnCreateActorButton() {
        var viewModel = DataContext as CreateActorViewModel;
        if (viewModel != null) {
            viewModel.CreateActor();
        }
    }

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null){
            HandlePropertyChanged(newContext_, "ShowPanel");
            HandlePropertyChanged(newContext_, "ActorTypeID");
        }else{
        }
    }

    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
        var viewModel = sender_ as CreateActorViewModel;
        if (viewModel != null){
            switch (property_ as string){
                case "ShowPanel":
                    {
                        _createActorPanel.SetActive(viewModel.ShowPanel);
                    }
                    break;
                case "ActorTypeID":
                    {
                        var actorTypeId = viewModel.ActorTypeID;
                        for (int i = 0; i < _actorTextList.Count; i++){
                            _actorTextList[i].gameObject.SetActive(i == actorTypeId);
                        }
                        _actorNameText.text = _actorNameList[actorTypeId];
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
