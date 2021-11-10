using UnityEngine;

namespace Bug.WeaponSystem
{
    public class DamageDisplay : MonoBehaviour
    {
        private float _remainingTime;

        private void Update()
        {
            transform.Translate(Vector3.up * Time.deltaTime);
            _remainingTime -= Time.deltaTime;
            if (_remainingTime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
