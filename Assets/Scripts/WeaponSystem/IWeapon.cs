using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bug.WeaponSystem
{
	public interface IWeapon : IPrimaryActionHandler, ISecondaryActionHandler, IReloadHandler { }
}
