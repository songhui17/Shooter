using UnityEngine;
using UnityEngine.SceneManagement;

public class ToyUI : MonoBehaviour {
    public void OnStartFightButton() {
        SceneManager.LoadScene("Prototype");
    }
}
