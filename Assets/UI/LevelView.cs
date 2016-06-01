using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Shooter;

public class LevelView : ViewBase {
    private static LevelView _lastSelectedLevelInfo;

    [SerializeField]
    ScrollRect _scrollRect;

    [SerializeField]
    Animator _animator;

    [SerializeField]
    private GameObject _infoPanel;

    [SerializeField]
    private Image _star1Image;

    [SerializeField]
    private Image _star2Image;

    [SerializeField]
    private Image _star3Image;

    [SerializeField]
    private Button _startButton;

    void Awake()
    {
        DataContext = gameObject.AddComponent<LevelViewModel>();
    }

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_) {
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null){
            HandlePropertyChanged(newContext_, "ActorLevelInfo");
            HandlePropertyChanged(newContext_, "CanFight");
        }else{
        }
    }

    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
        var viewModel = DataContext as LevelViewModel;
        switch (property_ as string){
            case "ActorLevelInfo":
                {
                    var levelInfo = viewModel.ActorLevelInfo;
                    _star1Image.color = levelInfo.star1 ? Color.white : Color.black;
                    _star2Image.color = levelInfo.star2 ? Color.white : Color.black;
                    _star3Image.color = levelInfo.star3 ? Color.white : Color.black;
                }
                break;
            case "CanFight":
                {
                    _startButton.interactable = viewModel.CanFight;
                }
                break;
            default:
                break;
        }
    }

    public void OnButton() {
        if (_infoPanel != null){
            var open = !_infoPanel.activeInHierarchy;
            if (open) {
                StartCoroutine(Open());
            }else{
                StartCoroutine(TriggerClose());
                StartCoroutine(Close());
            }
        }
    }

    IEnumerator Close(){
        while (true){
            if (!Opened) {
                if (_lastSelectedLevelInfo == this)
                    _lastSelectedLevelInfo = null;
                break;
            }
            yield return null;
        }
        yield return null;
    }

    enum OPEN_STATE {
        ClosePrev,
        SetPosition,
        Open,
        Done,
    }

    public bool Opened = false;
    public IEnumerator TriggerOpen() {
        _infoPanel.SetActive(true);
        if (!Opened && _animator != null){
            _animator.speed = 1.0f;
            _animator.SetBool("Open", true);
            while (true) {
                if (!_animator.IsInTransition(0)) {
                    var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                    if (stateInfo.IsName("LevelInfoOpen")){
                        break;
                    }
                }
                yield return null;
            }
        }
        Opened = true;
        yield return null;
    }

    public IEnumerator TriggerClose(float speed_ = 1.0f) {
        if (Opened && _animator != null){
            _animator.speed = speed_;
            _animator.SetBool("Open", false);
            while (true){
                if (!_animator.IsInTransition(0)) {
                    var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                    if (stateInfo.IsName("LevelInfoClose")){
                        break;
                    }
                }
                yield return null;
            }
        }
        _infoPanel.SetActive(false);
        yield return null;
        Opened = false;
    }

    void EnterSetPositionState(out float newHorizontalPosition,
                               out float hiddenLength, out int direction) {
        var rectTransform = GetComponent<RectTransform>();
        var viewRect = _scrollRect.GetComponent<RectTransform>().rect;
        var contentRect = _scrollRect.content.rect;

        var info = "ClosePrev -> SetPosition:\n";
        info += "rectTransform: " + rectTransform + "\n";
        info += "rectTransform.rect: " + rectTransform.rect + "\n";
        info += string.Format("viewRect: {0}\n", viewRect);
        info += string.Format("contentRect: {0}\n", contentRect);
        hiddenLength = contentRect.width - viewRect.width;
        info += string.Format("hiddenLength: {0}\n", hiddenLength);

        if (hiddenLength != 0){
            newHorizontalPosition = rectTransform.localPosition.x / hiddenLength;
            info += string.Format("newHorizontalPosition: {0}\n", newHorizontalPosition);
        }else{
            newHorizontalPosition = 0;
            info += string.Format("newHorizontalPosition: {0}\n", newHorizontalPosition);
        }
        direction = newHorizontalPosition > _scrollRect.horizontalNormalizedPosition ? 1 : -1;
    }

    IEnumerator Open() {
        OPEN_STATE openState = OPEN_STATE.ClosePrev;
        var newHorizontalPosition = 0.0f;
        var hiddenLength = 0.0f;
        var direction = 1;

        _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        if (_lastSelectedLevelInfo != null){
            StartCoroutine(_lastSelectedLevelInfo.TriggerClose(1.0f));
        }

        while (true){
            switch (openState){
                case OPEN_STATE.ClosePrev:
                    {
                        if (_lastSelectedLevelInfo == null || !_lastSelectedLevelInfo.Opened){
                            _lastSelectedLevelInfo = null;
                            // _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
                            openState = OPEN_STATE.SetPosition;
                            EnterSetPositionState(
                                out newHorizontalPosition, out hiddenLength, out direction);
                        }
                    }
                    break;
                case OPEN_STATE.SetPosition:
                    {
                        var speed = 400.0f;
                        speed *= direction;
                        var delta = speed * Time.deltaTime;
                        if (hiddenLength != 0){
                            delta /= hiddenLength;
                        }else{
                            delta = 0;
                        }
                        _scrollRect.horizontalNormalizedPosition += delta;

                        var diff = newHorizontalPosition - _scrollRect.horizontalNormalizedPosition;
                        var goPass = diff * direction <= 0;
                        diff = hiddenLength * diff;
                        var nearEnough = Mathf.Abs(diff) < 0.1f;
                        if (hiddenLength <= 0 || goPass || nearEnough) {
                            Debug.Log(string.Format("SetPosition -> Open:\n{0}: {1}\n{2}: {3}",
                                    "goPass", goPass, "nearEnough", nearEnough));
                            _scrollRect.horizontalNormalizedPosition = newHorizontalPosition;
                            openState = OPEN_STATE.Open;

                            StartCoroutine(TriggerOpen());
                        }
                    }
                    break;
                case OPEN_STATE.Open:
                    {
                        if (Opened){
                            bool retry = false;
                            if (hiddenLength < 0){
                                EnterSetPositionState(
                                        out newHorizontalPosition, out hiddenLength, out direction);
                                if (hiddenLength > 0){
                                    retry = true;
                                }
                            }

                            if (retry){
                                openState = OPEN_STATE.SetPosition;
                            }else{
                                _lastSelectedLevelInfo = this;

                                Debug.Log("Open -> Done");
                                _scrollRect.movementType = ScrollRect.MovementType.Elastic;
                                openState = OPEN_STATE.Done;
                            }
                        }
                    }
                    break;
            }

            if (openState == OPEN_STATE.Done) break;
            yield return null;
        }

        Debug.Log(string.Format("{0}: Open done", this));
    }
}
