using System.Collections;

namespace Bug.WeaponSystem
{
	public interface IReloadHandler
	{
		void ReloadRequested();

		void ReloadCancel();
	}
}
