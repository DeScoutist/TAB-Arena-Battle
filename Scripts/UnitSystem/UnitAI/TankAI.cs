using UnityEngine;

namespace UnitSystem.UnitAI
{
	public class TankAI : PlayerAI
	{
		protected override void Start()
		{
			base.Start();
		}

		protected override void Update()
		{
			base.Update();

			if (selectedTarget != null && !isTaskedToRun)
			{
				// Если у юнита есть цель и не было новых приказов, то юнит просто стоит и атакует
				// Реализация атаки
			}
		}

		protected override void HandleRightClick()
		{
			RaycastHit hitInfo;
			bool isHit = Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hitInfo);
			Unit hitUnit = isHit ? hitInfo.transform.GetComponent<Unit>() : null;

			if (hitUnit != null)
			{
				if (hitUnit.isEnemy)
				{
					// Если юнит является врагом, атакуем его
					MoveOrAttack(hitInfo);
					lineRenderer.startColor = lineRenderer.endColor = Color.red; // Измените цвет линии на красный
				}
				else
				{
					isTaskedToFollow = false;
					return;
				}
			}
			else
			{
				// Если юниту был дан приказ перемещаться, но цель остается прежней
				lineRenderer.startColor = lineRenderer.endColor = Color.green;
				isTaskedToFollow = false;
				MoveToPointSlowly();
			}
		}

		protected void MoveToPointSlowly()
		{
			float slowSpeed = speed / 2; // Медленная скорость, например, в два раза меньше обычной
			float distance;
			Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
			Plane plane = new Plane(Vector3.up, transform.position);

			if (plane.Raycast(ray, out distance))
			{
				positionToRunTo = ray.GetPoint(distance);
				isTaskedToRun = true;
			}

			transform.position = Vector3.MoveTowards(transform.position, positionToRunTo, slowSpeed * Time.deltaTime);
		}

		protected override void MoveOrAttack(RaycastHit unitHit)
		{
			Unit enemy = unitHit.transform.GetComponent<Unit>();
			if (enemy != null && enemy.isEnemy)
			{
				selectedTarget = enemy;
				isInCombat = true;
				isTaskedToFollow = true;
				if (TargetIsOutOfAttackRange())
				{
					MoveAgainstTarget();
				}
			}
		}
	}
}