using System.Collections;

namespace Bug.WeaponSystem
{
	public interface IPrimaryActionHandler
	{
		/// <summary>
		/// Whether this item is ready to handle a primary action.
		/// </summary>
		bool CanHandlePrimaryAction { get; }

		IEnumerator HandlePrimaryAction();
	}
}
