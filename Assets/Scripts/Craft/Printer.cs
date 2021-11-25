using UnityEngine;

namespace Bug.Craft
{
    public class Printer : MonoBehaviour
    {
        private Screen _screen;

        [SerializeField]
        private Transform _plateau;

        [SerializeField]
        private float _startPos, _stopPos;
        private float _prog, _timeGoal;

        private bool _goUp = true;

        private void Start()
        {
            _screen = GetComponent<Screen>();
            _screen.StartCrafting.AddListener(new(Animate));
            _screen.SlotEmptied.AddListener(new(ResetPrinter));
            ResetPrinter();
        }

        private void Update()
        {
            _prog += Time.deltaTime;
            var p = _plateau.transform.localPosition;
            if (_goUp)
            {
                _plateau.transform.localPosition = Vector3.Lerp(new Vector3(p.x, _startPos, p.z), new Vector3(p.x, _stopPos, p.z), _prog / _timeGoal); 
            }
            else
            {
                _plateau.transform.localPosition = Vector3.Lerp(new Vector3(p.x, _stopPos, p.z), new Vector3(p.x, _startPos, p.z), _prog / _timeGoal);
            }
        }

        private void ResetPrinter()
        {
            _timeGoal = 1f;
            _prog = 0f;
            _goUp = false;
        }

        private void Animate(float time)
        {
            _timeGoal = time;
            _prog = 0f;
            _goUp = true;
        }
    }
}
