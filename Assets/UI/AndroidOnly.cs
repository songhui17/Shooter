using UnityEngine;

public class AndroidOnly : MonoBehaviour {
    void Awake() {
#if UNITY_STANDALONE
        gameObject.SetActive(false);
#endif
    }
}
