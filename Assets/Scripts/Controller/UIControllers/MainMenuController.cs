using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller.UIControllers
{
    [RequireComponent(typeof(AudioSource))]
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private AudioClip clickEffect;
        private AudioSource effectSrc;

        private void Awake()
        {
            effectSrc = GetComponent<AudioSource>();
        }

        private IEnumerator ButtonClickSound()
        {
            effectSrc.PlayOneShot(clickEffect);
            yield return new WaitForSeconds(clickEffect.length);
        }

        private IEnumerator StartWrapper()
        {
            yield return ButtonClickSound();
            SceneManager.LoadScene(2);
        }

        private IEnumerator TutorialWrapper()
        {
            yield return ButtonClickSound();
            SceneManager.LoadScene(1);
        }
        
        private IEnumerator QuitWrapper()
        {
            yield return ButtonClickSound();
            Application.Quit();
        }
        
        public void StartButtonAction() => StartCoroutine(StartWrapper());
        public void QuitButtonAction() => StartCoroutine(QuitWrapper());
        public void TutorialButtonAction() => StartCoroutine(TutorialWrapper());
    }
}
