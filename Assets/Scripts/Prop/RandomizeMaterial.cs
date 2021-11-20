using UnityEngine;

namespace Bug.Prop
{
    public class RandomizeMaterial : MonoBehaviour
    {
        [SerializeField]
        private Material[] mats;

        private void Start()
        {
            GetComponent<Renderer>().material = mats[Random.Range(0, mats.Length)];
        }
    }

}