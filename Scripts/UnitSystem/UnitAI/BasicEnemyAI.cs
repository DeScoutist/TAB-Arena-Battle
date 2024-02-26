using System.Collections;
using System.Linq;
using AbilitySystem.Abilities;
using AbilitySystem.Authoring;
using UnityEngine;

namespace UnitSystem.UnitAI
{
	public class BasicEnemyAI : MonoBehaviour, IAI
	{
		public float speed = 5f;
		public float attackRadius = 4f;
		public float attackDamage = 0f;
		public float abilityCastInterval = 5f; // Интервал между попытками произнести заклинания
		public bool castedAbilityRecently = true;
		public bool isInCombat = false;
		public float awarenessRadius = 40f; // Радиус внимания
		private AbilityController _controller;
		public Unit target;
		protected Unit thisUnit;

		// TODO: При касте способности, устанавливать эту позицию в цель способности (НУЖНО РЕАЛИЗОВАТЬ ПРАВИЛЬНЫЙ АЛГОРИТМ ВЫБОРА ПОЗИЦИИ ДЛЯ СПЕЛЛА)
		public UnityEngine.Transform transform => base.transform;
		public Vector3 SpellTargetPosition { get; set; }
		public Quaternion SpellTargetRotation { get; set; }

		private Coroutine attackRoutine;
		public AbstractAbilitySpec currentAbility;
		public float stopDistance = 3f;

		[SerializeField] private AbilityController abilityController;

		private void Start()
		{
			thisUnit = GetComponent<Unit>();
			_controller = GetComponent<AbilityController>();
		
			foreach (var unit in FindObjectsOfType<Unit>())
			{
				unit.onThreatChanged += OnThreatChanged;
			}
		}

		void Update()
		{
			if (target == null)
			{
				target = FindClosestEnemy();
			}

			if (target == null)
				return;

			// Если враг в радиусе внимания, то начинается бой
			if (!isInCombat)
			{
				isInCombat = true;
				StartCoroutine(CastedRecently());
			}

			float distance = Vector3.Distance(transform.position, target.transform.position);

			bool canMoveAndRotate = currentAbility == null || !(thisUnit.isCasting);

			if (canMoveAndRotate)
			{
				MoveTowardsEnemy(target, distance);
				RotateTowardsEnemy(target);
			}

			if (distance <= attackRadius)
				InitiateAttackOnEnemy(target);

			if (currentAbility == null && !thisUnit.isCasting && !castedAbilityRecently)
			{
				TryActivateRandomAbility();
			}
		}

		private void OnThreatChanged(float newThreat, Unit dealer)
		{
			// Проверяем, является ли dealer врагом
			if (dealer.isEnemy != thisUnit.isEnemy)
			{
				// Если новая угроза больше текущей угрозы, обновляем цель
				if (newThreat > thisUnit.Threat)
				{
					// Сортируем словарь threatTable по убыванию угрозы
					var sortedThreatTable = thisUnit.threatTable.OrderByDescending(entry => entry.Value);

					// Если словарь не пуст, берем первый элемент как новую цель
					if (sortedThreatTable.Any())
					{
						target = sortedThreatTable.First().Key;
					}
				}
			}
		}
	
		private IEnumerator CastedRecently()
		{
			castedAbilityRecently = true;
			yield return new WaitForSeconds(abilityCastInterval);
			castedAbilityRecently = false;
		}

		private void MoveTowardsEnemy(Unit closestEnemy, float distance)
		{
			if (distance > stopDistance)
				transform.position = Vector3.MoveTowards(transform.position, closestEnemy.transform.position,
					speed * Time.deltaTime);
		}

		private void RotateTowardsEnemy(Unit closestEnemy)
		{
			Vector3 direction = closestEnemy.transform.position - this.transform.position;
			float yRotation = Quaternion.LookRotation(direction).eulerAngles.y;
			transform.rotation = Quaternion.Euler(0, yRotation, 0);
		}

		private void InitiateAttackOnEnemy(Unit closestEnemy)
		{
			if (target == null || target != closestEnemy || attackRoutine == null)
			{
				target = closestEnemy;

				if (attackRoutine != null)
					StopCoroutine(attackRoutine);

				attackRoutine = StartCoroutine(AttackTarget());
			}
		}

		private void TryActivateRandomAbility()
		{
			if (!_controller.abilitySpecs.Any())
				return;

			var availableAbilities = _controller.abilitySpecs.Where(a => a.CheckCooldown().TimeRemaining <= 0).ToList();

			if (!availableAbilities.Any())
				return;

			var random = UnityEngine.Random.Range(0, availableAbilities.Count);
			var selectedAbility = availableAbilities[random];

			// Проверяем, находится ли цель в пределах дистанции способности
			float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
			if (distanceToTarget > selectedAbility.Ability.Distance)
				return;

			SpellTargetPosition = target.transform.position;
			SpellTargetRotation = target.transform.rotation;

			random = 0; // Временно, пока не реализован выбор способности
		
			switch (random)
			{
				case 0:
					this.abilityController.UseAbility(random);
					break;
				case 1:
					SpellTargetPosition += new Vector3(Random.Range(-15, 15), 0, Random.Range(-15, 15));
					this.abilityController.UseAbility(random);
					break;
				case 2:
					SpellTargetPosition += new Vector3(Random.Range(-15, 15), 0, Random.Range(-15, 15));
					this.abilityController.UseAbility(random);
					break;
			}

			StartCoroutine(CastedRecently());
		}

		private Unit FindClosestEnemy()
		{
			Unit[] units = FindObjectsOfType<Unit>();
			Unit closest = null;
			Unit thisUnit = GetComponent<Unit>();
			float minDistance = Mathf.Infinity;

			foreach (Unit unit in units)
			{
				if (unit.isEnemy != thisUnit.isEnemy)
				{
					float distance = Vector3.Distance(transform.position, unit.transform.position);

					if (distance < minDistance && distance <= awarenessRadius)
					{
						closest = unit;
						minDistance = distance;
					}
				}
			}

			return closest;
		}

		private IEnumerator AttackTarget()
		{
			while (target != null && Vector3.Distance(transform.position, target.transform.position) <= attackRadius)
			{
				target.ChangeHealth(-attackDamage, thisUnit, target);
				yield return new WaitForSeconds(2f);
			}
		}
	}
}