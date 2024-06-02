using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileController : MonoBehaviour
    {
        private bool ready;

        private void OnTriggerExit(Collider other) => ready = true;

        private void OnTriggerEnter(Collider other)
        {
            if (ready) Destroy(gameObject);
        } 
    }
}