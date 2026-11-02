using UnityEngine;

public class GameExit : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Exit button clicked!");
        // Quit the game (works in builds; in Editor, it only stops Play mode)
        Application.Quit();

        // Optional: For debugging in Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
