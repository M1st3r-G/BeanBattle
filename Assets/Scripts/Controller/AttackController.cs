using UnityEngine;

namespace Controller
{
    public class AttackController : MonoBehaviour
    {
        //ComponentReferences
        [SerializeField] private GameObject projectilePrefab;
        private ParticleSystem closeCombatSystem;
        //Params
        [SerializeField] private float projectileSpeed;
        //Temps
        //Public

        private void Awake()
        {
            closeCombatSystem = GetComponent<ParticleSystem>();
        }

        public float Attack(Transform target)
        {
            Vector3 direction = target.transform.position - transform.position;
            LookTowards(direction.x, direction.z);

            float distance = Vector3.Distance(target.position, transform.position);
            
            Debug.Log($"Distance: {distance}");
            
            if (!(distance < 1.5f))
                return ShotProjectile(direction, distance);
            
            closeCombatSystem.Emit(30);
            return closeCombatSystem.main.startLifetime.constant;

        }

        private float ShotProjectile(Vector3 direction, float distance)
        {
            Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody>().velocity = direction.normalized * projectileSpeed;
            return distance / projectileSpeed;
        }
        
        private void LookTowards(float x, float y)
        {
            float rotInDeg = - Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, rotInDeg, 0);
        }
    }
}