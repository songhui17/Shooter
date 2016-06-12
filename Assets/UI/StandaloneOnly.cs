using UnityEngine;

public class StandaloneOnly : MonoBehaviour {
    void Awake() {
#if UNITY_ANDROID
        gameObject.SetActive(false);
#endif
    }
}
