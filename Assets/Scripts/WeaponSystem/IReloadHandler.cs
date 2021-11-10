using System.Collections;

namespace Bug.WeaponSystem
{
	public interface IReloadHandler
	{
		bool CanReload { get; }

		IEnumerator HandleReload();
	}
}
