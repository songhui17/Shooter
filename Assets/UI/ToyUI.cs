using UnityEngine;
using UnityEngine.SceneManagement;

public class ToyUI : MonoBehaviour {
    public Animator _mainScreen;
    public Animator _chapterInfoScreen;

    public void OnStartFightButton() {
        SceneManager.LoadScene("Prototype");
    }

    public void OnLevelButton() {
        _mainScreen.SetBool("Open", false);
        _chapterInfoScreen.SetBool("Open", true);
    }

    public void OnBackButton() {
    }
}
