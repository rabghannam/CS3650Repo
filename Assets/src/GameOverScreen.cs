using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public GameObject panel;

    private void Start() { if (panel != null) panel.SetActive(false); }

    public void show()
    {
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;                 // stop gameplay; player cannot move or fire
    }

    public void render() { /* Canvas panel handles rendering */ }
}