using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD60Tec.Menu
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _menu;

        public void GoToMenu()
        {
            _menu.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
