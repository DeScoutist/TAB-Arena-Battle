using UnityEngine;

namespace UnitSystem.UnitAI
{
	public class DpsAI : PlayerAI
	{
		private Vector3 leftStrafePosition;
		private Vector3 rightStrafePosition;
		private bool isStrafingRight = true;

		protected override void Start()
		{
			base.Start();
		}

		protected override void Update()
		{
			base.Update();

			if (selectedTarget != null)
			{
				// StrafeAroundTarget();
			}
		}
	
		protected override void RotateTowardsSelectedTarget()
		{
			if (selectedTarget == null) return;

			Vector3 direction = selectedTarget.transform.position - this.transform.position;
			float yRotation = Quaternion.LookRotation(direction).eulerAngles.y;
			transform.rotation = Quaternion.Euler(0, yRotation, 0);

			// Обновляем позиции для стрейфа
			Vector3 strafeDirection = new Vector3(-direction.z, 0, direction.x);
			float strafeDistance = 1f; // Расстояние стрейфа
			leftStrafePosition = transform.position - strafeDirection * strafeDistance;
			rightStrafePosition = transform.position + strafeDirection * strafeDistance;
		}

		protected void StrafeAroundTarget()
		{
			if (!isTaskedToFollow) return;

			// Если цель перемещается или персонаж находится вне радиуса атаки, следуем за целью
			if (Vector3.Distance(transform.position, selectedTarget.transform.position) > attackRadius)
			{
				MoveAgainstTarget();
				return;
			}

			// Если цель не перемещается и персонаж находится в радиусе атаки, стрейфимся
			if (isStrafingRight)
			{
				transform.position = Vector3.MoveTowards(transform.position, rightStrafePosition, speed * Time.deltaTime);
				if (transform.position == rightStrafePosition)
				{
					isStrafingRight = false;
				}
			}
			else
			{
				transform.position = Vector3.MoveTowards(transform.position, leftStrafePosition, speed * Time.deltaTime);
				if (transform.position == leftStrafePosition)
				{
					isStrafingRight = true;
				}
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
					// Если юнит является союзником, то ничего не делаем
					return;
				}
			}
			else
			{
				// Если юниту был дан приказ перемещаться
				MoveToPoint();
				isTaskedToFollow = false;
				lineRenderer.startColor = lineRenderer.endColor = Color.green;
			}
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