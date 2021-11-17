using System.Collections;

namespace Bug.WeaponSystem
{
	public interface IPrimaryActionHandler
	{
		void PrimaryActionBegin();

		void PrimaryActionEnd();
	}
}
