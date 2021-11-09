using System.Collections;

namespace Bug.WeaponSystem
{
	public interface ISecondaryActionHandler
	{
		bool CanHandleSecondaryAction { get; }

		IEnumerator HandleSecondaryAction();
	}
}
