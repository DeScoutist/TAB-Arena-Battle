using UnityEngine;
using UnitSystem;

public class TankAI : PlayerAI
{
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
				// Если юнит является союзником, то ничего не делаем
				return;
			}
		}
		else
		{
			MoveToPoint();
		}
	}
}