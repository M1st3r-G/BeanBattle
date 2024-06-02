using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller.UIControllers
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PauseController : MonoBehaviour
    {
        private CanvasGroup _group;
        private bool restoreBuffer;
        
        private void Awake()
        {
            _group = GetComponent<CanvasGroup>();
            SetVisible(false);
        }

        public void TogglePause(bool restoreAfter)
        {
            if (_group.alpha > 0.5f)
            {
                SetVisible(false);
                if (restoreBuffer) CustomInputManager.Instance.EnableInput();
            }
            else
            {
                CustomInputManager.Instance.DisableInput();
                restoreBuffer = restoreAfter;
                SetVisible(true);
            }
        }

        private void SetVisible(bool state)
        {
            _group.alpha = state ? 1f : 0f;
            _group.interactable = _group.blocksRaycasts = state;
        }
        
        public void MenuButtonAction() => SceneManager.LoadScene(0);
        public void ContinueButtonAction() => TogglePause(false);
    }
}