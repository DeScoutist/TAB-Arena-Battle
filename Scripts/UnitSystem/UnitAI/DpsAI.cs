using System.Collections;
using UnityEngine;
using UnitSystem;

public class DpsAI : PlayerAI
{
	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();

		if (selectedTarget != null)
		{
			StrafeAroundTarget();
		}
	}

	protected void StrafeAroundTarget()
	{
		if (!isTaskedToFollow) return;
		
		// Расстояние от цели, на котором должен находиться юнит
		float distanceFromTarget = 5f;

		// Минимальное расстояние от босса
		float minDistanceFromBoss = 1f;

		// Скорость стрейфа
		float strafeSpeed = 5f;

		// Расчет новой позиции
		Vector3 targetPosition = selectedTarget.transform.position; // позиция цели
		Vector3 directionToTarget = (targetPosition - transform.position).normalized; // направление к цели
		Vector3 strafeDirection = new Vector3(-directionToTarget.z, 0, directionToTarget.x); // направление стрейфа (перпендикулярно направлению к цели)

		// Расчет позиции для стрейфа с использованием функции Mathf.Sin для создания движения влево-вправо
		Vector3 strafePosition = targetPosition + directionToTarget * distanceFromTarget + strafeDirection * Mathf.Sin(Time.time * strafeSpeed);

		// Проверка, что юнит находится на безопасном расстоянии от босса
		if (Vector3.Distance(transform.position, targetPosition) > minDistanceFromBoss)
		{
			// Перемещение юнита к расчетной позиции
			transform.position = Vector3.MoveTowards(transform.position, strafePosition, speed * Time.deltaTime);
		}

		// Поворот юнита к цели
		Quaternion toTarget = Quaternion.LookRotation(targetPosition - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, toTarget, speed * Time.deltaTime);
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
			if (TargetIsOutOfAttackRange())
			{
				MoveAgainstTarget();
			}
		}
	}
}