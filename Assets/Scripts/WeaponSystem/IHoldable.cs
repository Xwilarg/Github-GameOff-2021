using UnityEngine;

namespace Bug.WeaponSystem
{
	public interface IHoldable
	{
		public GameObject GetHolder();
		public void HoldBegin(GameObject holder);
		public void HoldEnd(GameObject holder);
	}
}
