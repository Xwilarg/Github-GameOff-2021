using UnityEngine;
using Bug.WeaponSystem;
using TMPro;

namespace Bug.UI
{
	public class AmmoDisplay : MonoBehaviour
	{
		[SerializeField] private TMP_Text _label;
		[SerializeField] private Firearm _firearm;


		private void Update()
		{
			_label.text = _firearm.AmmoCount.ToString();
		}

		private void Reset()
		{
			_label = GetComponent<TMP_Text>();
		}
	}
}
