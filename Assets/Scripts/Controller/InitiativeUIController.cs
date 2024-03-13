using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    public class InitiativeUIController : MonoBehaviour
    {
        //Debug
        [SerializeField] private CharController test;
        
        private BannerController[] _banner = new BannerController[12];

        private void Awake()
        {
            _banner = GetComponentsInChildren<BannerController>(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                UpdateUI(new List<CharController> {test});
            }
        }

        private void UpdateUI(List<CharController> currentOrder)
        {
            for (int i = 0; i < currentOrder.Count; i++)
            {
                _banner[i].gameObject.SetActive(true);
                _banner[i].SetTo(currentOrder[i]);
            }

            for (int i = currentOrder.Count; i < 12; i++)
            {
                _banner[i].gameObject.SetActive(false);
            }
            
            // Fix Border Height
        }
    }
}