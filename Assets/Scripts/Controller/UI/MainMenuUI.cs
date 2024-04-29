using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public void StartButtonAction() => SceneManager.LoadScene(1);
        public void QuitButtonAction() => Application.Quit();
    }
}
