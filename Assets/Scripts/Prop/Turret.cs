using UnityEngine;

namespace Bug.Prop
{
    public class Turret : MonoBehaviour
    {
        [SerializeField]
        private GameObject _head;

        [SerializeField]
        private float _maxRot;

        [SerializeField]
        private float _rotSpeed;

        private Placable _placable;
        private bool _goLeft;

        private void Start()
        {
            _placable = GetComponent<Placable>();
        }

        private void Update()
        {
            if (_placable.IsPlaced)
            {
                if (_goLeft)
                {
                    _head.transform.Rotate(0f, -1f * Time.deltaTime * _rotSpeed, 0f);
                    if (_head.transform.rotation.y < -_maxRot)
                    {
                        _goLeft = false;
                    }
                }
                else
                {
                    _head.transform.Rotate(0f, 1f * Time.deltaTime * _rotSpeed, 0f);
                    if (_head.transform.rotation.y > _maxRot)
                    {
                        _goLeft = true;
                    }
                }
            }
        }
    }
}
