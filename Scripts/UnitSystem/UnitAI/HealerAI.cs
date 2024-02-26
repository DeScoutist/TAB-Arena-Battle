using System.Collections.Generic;
using System.Linq;
using UI;

namespace UnitSystem.UnitAI
{
	public class HealerAI : PlayerAI
	{
		private const float HEAL_AMOUNT = 20f;
		private Unit healTarget;
		private SpawnUnit spawnUnit;
		private float manaPool = 100f; // Манапул
	
		protected override void Start()
		{
			base.Start();

			spawnUnit = FindObjectOfType<SpawnUnit>();
		}

		protected override void Update()
		{
			base.Update();

			if (!isTaskedToRun && !this.transform.GetComponent<UnitUI>().IsCasting())
			{
				SelectHealTarget();
				// Dispel();
			}

			if (manaPool <= 0)
			{
				AttackWhenManaPoolIsDepleted();
			}
		}

		private List<Unit> GetGroupMembers()
		{
			return spawnUnit.units;
		}

		private void SelectHealTarget()
		{
			List<Unit> groupMembers = GetGroupMembers();

			Unit mostInjuredUnit = groupMembers.FirstOrDefault(unit => unit.HealthPercentage <= 20 && unit != null);
			if (mostInjuredUnit != null)
			{
				healTarget = mostInjuredUnit;
				HealUnit(healTarget);
				return;
			}

			Unit tank = groupMembers.FirstOrDefault(unit => unit.isTank && unit.HealthPercentage <= 70);
			if (tank != null)
			{
				healTarget = tank;
				HealUnit(healTarget);
				return;
			}

			Unit injuredUnit = groupMembers.FirstOrDefault(unit => unit.HealthPercentage < 100);
			if (injuredUnit != null)
			{
				healTarget = injuredUnit;
				HealUnit(healTarget);
				return;
			}

			healTarget = null;
		}

		private void Dispel()
		{
			// Реализация снятия заклинания с союзника каждые 4 секунды
			// Это псевдокод, вам нужно добавить реальную реализацию
		}

		private void AttackWhenManaPoolIsDepleted()
		{
			// Реализация атаки, когда манапул исчерпан
			// Это псевдокод, вам нужно добавить реальную реализацию
		}

		private void HealUnit(Unit unit)
		{
			selectedTarget = unit;
			if (abilityController != null && abilityController.Abilities.Length > 0)
			{
				abilityController.UseAbility(0);
			}
			// manaPool -= HEAL_AMOUNT; // Уменьшаем манапул на количество затраченного на лечение маны
		}
	}
}