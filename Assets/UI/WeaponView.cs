using UnityEngine;
using UnityEngine.UI;

public class WeaponView : ViewBase {
    [SerializeField]
    private RayWeapon _viewModel;

    [SerializeField]
    private Text _ammoText;

    void Awake() {
        DataContext = _viewModel;
    }

    protected override void OnDataContextChanged(
            INotifyPropertyChanged newContext_){
        base.OnDataContextChanged(newContext_);

        if (newContext_ != null){
            HandlePropertyChanged(newContext_, "TotalBullet");
            HandlePropertyChanged(newContext_, "LeftBullet");
        }else{
        }
    }

    protected override void HandlePropertyChanged(
            INotifyPropertyChanged sender_, object property_){
        var viewModel = sender_ as RayWeapon;
        switch (property_ as string){
            case "TotalBullet":
                {
                    var format = StringTable.Value("HUD.Ammo.Format");
                    _ammoText.text = string.Format(
                        format, viewModel.LeftBullet, viewModel.TotalBullet);
                }
                break;
            case "LeftBullet":
                {
                    var format = StringTable.Value("HUD.Ammo.Format");
                    _ammoText.text = string.Format(
                        format, viewModel.LeftBullet, viewModel.TotalBullet);
                }
                break;
            default:
                break;
        }
    }
}
