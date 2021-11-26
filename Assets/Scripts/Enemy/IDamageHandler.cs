namespace Bug.Enemy
{
	public interface IDamageHandler
	{
		public void TakeDamage(float delta);

		public void Heal(float delta);
	}
}
