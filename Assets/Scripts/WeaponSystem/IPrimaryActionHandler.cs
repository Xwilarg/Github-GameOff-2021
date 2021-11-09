using System.Collections;

namespace Bug.WeaponSystem
{
	public interface IPrimaryActionHandler
	{
		bool CanHandlePrimaryAction { get; }

		IEnumerator HandlePrimaryAction();
	}
}
