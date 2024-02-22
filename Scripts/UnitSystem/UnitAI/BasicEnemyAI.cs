using System.Collections;
using System.Linq;
using AbilitySystem.Authoring;
using UnitSystem;
using UnitSystem.UnitAI;
using UnityEngine;

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
	public Unit Target;
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
	}

	void Update()
	{
		Unit closestEnemy = FindClosestEnemy();

		if (closestEnemy == null)
			return;

		// Если враг в радиусе внимания, то начинается бой
		if (!isInCombat)
		{
			isInCombat = true;
			StartCoroutine(CastedRecently());
		}

		float distance = Vector3.Distance(transform.position, closestEnemy.transform.position);

		bool canMoveAndRotate = currentAbility == null || !(thisUnit.isCasting);

		if (canMoveAndRotate)
		{
			MoveTowardsEnemy(closestEnemy, distance);
			RotateTowardsEnemy(closestEnemy);
		}

		if (distance <= attackRadius)
			InitiateAttackOnEnemy(closestEnemy);

		if (currentAbility == null && !thisUnit.isCasting && !castedAbilityRecently)
		{
			ActivateRandomAbility();
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
		if (Target == null || Target != closestEnemy || attackRoutine == null)
		{
			Target = closestEnemy;

			if (attackRoutine != null)
				StopCoroutine(attackRoutine);

			attackRoutine = StartCoroutine(AttackTarget());
		}
	}

	private void ActivateRandomAbility()
	{
		if (!_controller.abilitySpecs.Any())
			return;

		var availableAbilities = _controller.abilitySpecs.Where(a => a.CheckCooldown().TimeRemaining <= 0).ToList();

		if (!availableAbilities.Any())
			return;

		var random = UnityEngine.Random.Range(0, availableAbilities.Count);
		
		SpellTargetPosition = Target.transform.position;
		SpellTargetRotation = Target.transform.rotation;

		random = 2;
		switch (random)
		{
			case 0:
				this.abilityController.UseAbility(random);
				break;
			case 1:
				SpellTargetPosition += new Vector3(Random.Range(0, 15), 0, Random.Range(0, 15));
				this.abilityController.UseAbility(random);
				break;
			case 2:
				SpellTargetPosition += new Vector3(Random.Range(0, 15), 0, Random.Range(0, 15));
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
		while (Target != null && Vector3.Distance(transform.position, Target.transform.position) <= attackRadius)
		{
			Target.ChangeHealth(-attackDamage, thisUnit, Target);
			yield return new WaitForSeconds(2f);
		}
	}
}