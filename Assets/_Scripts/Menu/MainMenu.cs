using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD60Tec.Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _settings;

        public void StartGame()
        {
            SceneManager.LoadScene("Gameplay");
        }
        public void GoToSettings()
        {
            _settings.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
