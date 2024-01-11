using UnityEngine;

namespace UnitSystem
{
	public class UnitController : MonoBehaviour
	{
		public float health = 100f;

		private void Start()
		{
		}

		public void TakeDamage(float damage)
		{
			health -= damage;
		}
	}
}