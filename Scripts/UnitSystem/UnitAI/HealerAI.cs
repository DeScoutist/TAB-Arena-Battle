using UnitSystem;
using UnityEngine;

public class HealerAI : PlayerAI
{
	private const float HEAL_AMOUNT = 20f;

	protected override void HandleRightClick()
	{
		RaycastHit hitInfo;
		bool isHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
		Unit hitUnit = isHit ? hitInfo.transform.GetComponent<Unit>() : null;

		if (hitUnit != null)
		{
			if (hitUnit.isEnemy)
			{
				// Если юнит является врагом, атакуем его
				MoveOrAttack(hitInfo);
			}
			else
			{
				// Если юнит является союзником, лечим его
				HealUnit(hitUnit);
			}
		}
		else
		{
			MoveToPoint();
		}
	}

	private void HealUnit(Unit unit)
	{
		unit.ChangeHealth(HEAL_AMOUNT);
	}
}