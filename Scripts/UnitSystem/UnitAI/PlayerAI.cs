using System.Collections;
using System.Linq;
using AbilitySystem;
using GameplayTag.Authoring;
using UnityEngine;
using UnitSystem;

public class PlayerAI : MonoBehaviour
{
	protected const float WAIT_ATTACK_DURATION = 2f;
	protected const float LINE_RENDERER_WIDTH = 0.1f;
	protected const string BOSS_TAG = "BOSS_TAG";
	protected const string DEFAULT_SHADER = "Sprites/Default";

	[SerializeField] protected float speed = 5f;
	[SerializeField] protected float attackRadius = 4f;
	[SerializeField] protected float attackDamage = 10f;
	[SerializeField] protected float stopDistance = 3f;

	protected Coroutine attackRoutine;
	protected bool isTaskedToRun = false;
	protected bool isTaskedToFollow = false;
	protected bool isAttackOnCooldown = false;
	protected Vector3 positionToRunTo;
	protected LineRenderer lineRenderer;
	protected Unit selectedTarget;
	protected bool isInCombat = false;
	protected Camera playerCamera;
	protected Unit thisUnit;
	protected float lastAttackTime = 0f; // Добавьте это в начало класса
	protected AbilitySystemCharacter character; // Ссылка на AbilitySystemCharacter

	protected GameplayTagScriptableObject stunTag;
	

	protected virtual void Start()
	{
		thisUnit = this.GetComponent<Unit>();
		InitializeLineRenderer();
		playerCamera = GameObject.FindWithTag("PLAYER_CAMERA").GetComponent<Camera>();
		character = GetComponent<AbilitySystemCharacter>(); // Получаем компонент AbilitySystemCharacter

		LoadTags();
	}

	protected void LoadTags()
	{
		// Инициализируем stunTag
		stunTag = Resources.Load<GameplayTagScriptableObject>("Prefabs/Tags/CharacterStates/Stunned");
	}
	
	protected void InitializeLineRenderer()
	{
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find(DEFAULT_SHADER));
		lineRenderer.startColor = lineRenderer.endColor = Color.green;
		lineRenderer.startWidth = lineRenderer.endWidth = LINE_RENDERER_WIDTH;
	}

	protected virtual void Update()
	{
		ManageMovement();

		if (IsPlayerControlled())
		{
			Debug.Log("123123123");
			// Если персонаж оглушен, игнорируем ввод игрока
			if (character.AppliedGameplayEffects.Any(effect => effect.spec.GameplayEffect.gameplayEffectTags.GrantedTags.Contains(stunTag)))
			{
				Debug.Log("Stunned");
				return;
			}
			
			HandlePlayerInput();

			if (isTaskedToRun)
			{
				lineRenderer.enabled = true;
			}
		}
		else
		{
			HandleAITasks();
		}

		if (isInCombat)
		{
			if (attackRoutine == null)
			{
				attackRoutine = StartCoroutine(TryAttackTarget());
			}
		}
	}

	protected bool IsPlayerControlled()
	{
		return UnitSelection.selectedUnit == this.GetComponent<Unit>();
	}

	private IEnumerator AttackCooldown(float cooldownTime)
	{
		yield return new WaitForSeconds(cooldownTime);
		isAttackOnCooldown = false;
	}

	protected virtual void HandlePlayerInput()
	{
		if (Input.GetMouseButtonDown(1))
		{
			HandleRightClick();
		}
	}

	protected virtual void HandleRightClick()
	{
		RaycastHit hitInfo;
		bool isHit = Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hitInfo);
		Unit hitUnit = isHit ? hitInfo.transform.GetComponent<Unit>() : null;

		if (hitUnit != null && hitUnit.isEnemy)
		{
			MoveOrAttack(hitInfo);
			lineRenderer.startColor = lineRenderer.endColor = Color.red; // Измените цвет линии на красный
		}
		else
		{
			MoveToPoint();
			isTaskedToFollow = false;
			lineRenderer.startColor = lineRenderer.endColor = Color.green;
		}
	}

	protected virtual void MoveOrAttack(RaycastHit unitHit)
	{
		Unit enemy = unitHit.transform.GetComponent<Unit>();
		if (enemy != null && enemy.isEnemy)
		{
			// Установите выбранную цель
			isTaskedToFollow = true;
			selectedTarget = enemy;
			isInCombat = true;
			if (TargetIsOutOfAttackRange())
			{
				MoveAgainstTarget();
			}
		}
	}

	protected virtual void MoveToPoint()
	{
		// Получаем текущий тег персонажа
		// GameplayTag.Authoring.GameplayTagScriptableObject currentTag = thisUnit.GetCurrentTag();
		//
		// // Проверяем, является ли текущий тег одним из тегов, запрещающих движение
		// if (currentTag == thisUnit.stunnedTag || currentTag == thisUnit.immobilizedTag || currentTag == thisUnit.astralTag)
		// {
		// 	// Если персонаж обладает тегом, запрещающим движение, прекращаем выполнение метода
		// 	return;
		// }

		float distance;
		Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
		Plane plane = new Plane(Vector3.up, transform.position);

		if (plane.Raycast(ray, out distance))
		{
			positionToRunTo = ray.GetPoint(distance);
			isTaskedToRun = true;
		}
	}

	protected virtual void HandleAITasks()
	{
		lineRenderer.enabled = false;
	}

	protected void ManageMovement()
	{
		if (isTaskedToRun)
		{
			if (selectedTarget && isTaskedToFollow)
			{
				positionToRunTo = selectedTarget.transform.position;
			}

			transform.position = Vector3.MoveTowards(transform.position, positionToRunTo, speed * Time.deltaTime);
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, positionToRunTo); // Set the end position of the line
			lineRenderer.enabled = true;

			if (Vector3.Distance(transform.position, positionToRunTo) <= 1)
			{
				isTaskedToRun = false;
				lineRenderer.enabled = false;
			}
		}
	}

	protected bool TargetIsInRange()
	{
		return selectedTarget != null &&
		       Vector3.Distance(transform.position, selectedTarget.transform.position) <= attackRadius;
	}

	protected IEnumerator TryAttackTarget()
	{
		if (isAttackOnCooldown) yield return null;

		while (selectedTarget != null &&
		       Vector3.Distance(transform.position, selectedTarget.transform.position) <= attackRadius)
		{
			if (!isAttackOnCooldown)
			{
				isAttackOnCooldown = true;
				selectedTarget.GetComponent<Unit>().ChangeHealth(-attackDamage);
				StartCoroutine(AttackCooldown(WAIT_ATTACK_DURATION));
			}

			yield return null;
		}

		attackRoutine = null;
	}

	protected bool TargetIsOutOfAttackRange()
	{
		return Vector3.Distance(transform.position, selectedTarget.transform.position) > attackRadius;
	}

	protected void MoveAgainstTarget()
	{
		positionToRunTo = selectedTarget.transform.position;
		isTaskedToRun = true;
	}
}