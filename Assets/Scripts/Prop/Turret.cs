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

        private float _baseRot = -1f, _prog;

        private void Start()
        {
            _placable = GetComponent<Placable>();
        }

        private void Update()
        {
            if (_placable.IsPlaced)
            {
                if (_baseRot == -1f)
                {
                    _baseRot = transform.rotation.eulerAngles.y;
                }
                if (_goLeft)
                {
                    _prog -= Time.deltaTime * _rotSpeed;
                    _head.transform.rotation = Quaternion.AngleAxis(_baseRot + _prog, Vector3.up);
                    if (_prog < -_maxRot)
                    {
                        _goLeft = false;
                    }
                }
                else
                {
                    _prog += Time.deltaTime * _rotSpeed;
                    _head.transform.rotation = Quaternion.AngleAxis(_baseRot + _prog, Vector3.up);
                    if (_prog > _maxRot)
                    {
                        _goLeft = true;
                    }
                }
            }
        }
    }
}
