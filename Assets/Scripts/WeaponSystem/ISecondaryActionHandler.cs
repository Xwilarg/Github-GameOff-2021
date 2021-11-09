using System.Collections;

namespace Bug.WeaponSystem
{
	public interface ISecondaryActionHandler
	{
		/// <summary>
		/// Whether this item is ready to handle a secondary action.
		/// </summary>
		bool CanHandleSecondaryAction { get; }

		IEnumerator HandleSecondaryAction();
	}
}
