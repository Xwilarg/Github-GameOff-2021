using System.Collections;

namespace Bug.WeaponSystem
{
	public interface IReloadHandler
	{
		void ReloadRequested();

		bool IsReloading();

		void ReloadCancel();
	}
}
