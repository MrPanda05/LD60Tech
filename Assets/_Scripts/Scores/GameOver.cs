using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD60Tec.Score
{
    public class GameOver : MonoBehaviour
    {
        public void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        public void Quit()
        {
            // 1. If playing inside the Unity Editor, stop the play mode
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // 2. If running a standalone built game, close the application
        Application.Quit();
#endif
        }
    }
}
