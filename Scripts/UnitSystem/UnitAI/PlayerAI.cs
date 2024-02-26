using System.Collections;
using System.Linq;
using AbilitySystem.Abilities;
using AbilitySystem.Components;
using GameplayTags.Authoring;
using UI;
using UnityEngine;

namespace UnitSystem.UnitAI
{
	public class PlayerAI : MonoBehaviour, IAI
	{
		protected const float WAIT_ATTACK_DURATION = 2f;
		protected const float MIN_DISTANCE_TO_TARGET = 1f;
		protected const float LINE_RENDERER_WIDTH = 0.1f;
		protected const string BOSS_TAG = "BOSS_TAG";
		protected const string DEFAULT_SHADER = "Sprites/Default";

		[SerializeField] protected float speed = 5f;
		[SerializeField] protected float attackRadius = 4f;
		[SerializeField] protected float attackDamage = 10f;
		[SerializeField] protected float stopDistance = 3f;

		public UnityEngine.Transform transform => base.transform;
		public Vector3 SpellTargetPosition { get; set; }
		public Quaternion SpellTargetRotation { get; set; }

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
		protected UnitUI thisUnitUI;
		protected float lastAttackTime = 0f; // Добавьте это в начало класса
		protected AbilitySystemCharacter character; // Ссылка на AbilitySystemCharacter
		protected AbilityController abilityController; // Ссылка на AbilityController

		// TAGS
		protected GameplayTagScriptableObject stunTag;

		private UnitFrameController UnitFrameController;

		public Unit SelectedTarget
		{
			get { return selectedTarget; }
			protected set { selectedTarget = value; }
		}

		protected virtual void Start()
		{
			thisUnit = this.GetComponent<Unit>();
			thisUnitUI = this.GetComponent<UnitUI>();
			InitializeLineRenderer();
			playerCamera = GameObject.FindWithTag("PLAYER_CAMERA").GetComponent<Camera>();
			character = GetComponent<AbilitySystemCharacter>(); // Получаем компонент AbilitySystemCharacter
			abilityController = GetComponent<AbilityController>(); // Получаем компонент AbilityController

			// Находим объект UnitFrameController на сцене и получаем его компонент UnitFrameController
			UnitFrameController = GameObject.Find("SelectedUnitFrame").GetComponent<UnitFrameController>();

			LoadTags();
		}

		protected void LoadTags()
		{
			// Инициализируем stunTag
			stunTag = UnityEngine.Resources.Load<GameplayTagScriptableObject>("Prefabs/Tags/CharacterStates/Stunned");
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
			if (thisUnit.isDead)
			{
				return;
			}

			ManageMovement();
			RotateTowardsSelectedTarget();

			if (IsPlayerControlled())
			{
				// Если персонаж оглушен, игнорируем ввод игрока
				if (character.AppliedGameplayEffects.Any(effect =>
					    effect.spec.GameplayEffect.gameplayEffectTags.GrantedTags.Contains(stunTag)))
				{
					// Debug.Log("Stunned");
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

			// Если этот персонаж является выбранным, обновляем UI
			if (UnitSelection.selectedUnit == thisUnit)
			{
				// Обновляем UI для выбранного юнита
				if (UnitSelection.selectedUnit != null)
				{
					UnitFrameController.SelectedUnitFrame.SetActive(true);
					UnitFrameController.SelectedUnitIcon.sprite =
						thisUnit.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite;
					UnitFrameController.SelectedUnitName.text = thisUnit.name;
					UnitFrameController.SelectedUnitHealthBar.fillAmount = thisUnitUI.HealthBar.fillAmount;
					UnitFrameController.SelectedUnitCastBar.fillAmount = thisUnitUI.CastBar.fillAmount;
				}
				else
				{
					UnitFrameController.SelectedUnitFrame.SetActive(false);
				}

				// Обновляем UI для цели выбранного юнита
				if (selectedTarget != null)
				{
					UnitFrameController.TargetedUnitFrame.SetActive(true);
					UnitFrameController.TargetedUnitIcon.sprite =
						selectedTarget.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite;
					UnitFrameController.TargetedUnitName.text = selectedTarget.name;
					UnitFrameController.TargetedUnitHealthBar.fillAmount =
						selectedTarget.GetComponent<UnitUI>().HealthBar.fillAmount;
					UnitFrameController.TargetedUnitCastBar.fillAmount =
						selectedTarget.GetComponent<UnitUI>().CastBar.fillAmount;
				}
				else
				{
					UnitFrameController.TargetedUnitFrame.SetActive(false);
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

		protected virtual void RotateTowardsSelectedTarget()
		{
			if (selectedTarget == null) return;

			Vector3 direction = selectedTarget.transform.position - this.transform.position;
			float yRotation = Quaternion.LookRotation(direction).eulerAngles.y;
			transform.rotation = Quaternion.Euler(0, yRotation, 0);
		}

		protected virtual void MoveToPoint()
		{
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
				if (selectedTarget != null && isTaskedToFollow)
				{
					Vector3 directionToTarget = (selectedTarget.transform.position - transform.position).normalized;
					positionToRunTo = selectedTarget.transform.position - directionToTarget * MIN_DISTANCE_TO_TARGET;
				}

				UnitUI unitUI = GetComponent<UnitUI>();
				if (unitUI != null && unitUI.IsCasting())
				{
					unitUI.CancelCasting();
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
					selectedTarget.GetComponent<Unit>().ChangeHealth(-attackDamage, thisUnit, selectedTarget);
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
}