using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocaleText : MonoBehaviour {
    [SerializeField]
    string _path = "Default";

    void SetLocale() {
        if (!Application.isPlaying) {
            StringTable.Instance.Reload();
        }

        var text = GetComponent<Text>();
        if (text != null) {
            text.text = StringTable.Instance.Get(_path, this);
        }
    }

    void Start() {
        SetLocale();
    }
}

