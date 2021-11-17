using System.Collections;

namespace Bug.WeaponSystem
{
	public interface ISecondaryActionHandler
	{
		void SecondaryActionBegin();

		void SecondaryActionEnd();
	}
}
