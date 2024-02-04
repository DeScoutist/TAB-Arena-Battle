using System.Collections;
using AbilitySystem.Authoring;
using UnitSystem;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
	private AbstractAbilitySpec currentAbility;
	private float castStartTime;
	private bool isCasting;
	private Unit unit; // Ссылка на персонажа

	public Image HealthBar; // Полоска здоровья
	// public GameObject DebuffIconPrefab; // Префаб иконки дебаффа
	// public Transform DebuffContainer; // Контейнер для иконок дебаффов

	private void Start()
	{
		// Получаем ссылку на персонажа
		unit = GetComponent<Unit>();

		// Подписываемся на событие изменения здоровья
		// unit.onHealthChanged += UpdateHealthBar;
	}

	private void UpdateHealthBar(float healthPercentage)
	{
		// Устанавливаем заполненность полоски здоровья
		HealthBar.fillAmount = healthPercentage;
	}
	
	public void StartCasting(AbstractAbilitySpec ability)
	{
		currentAbility = ability;
		castStartTime = Time.time;
		isCasting = true;
	}

	public void Update()
	{
		if (isCasting && Time.time - castStartTime >= currentAbility.Ability.CastTime)
		{
			StartCoroutine(CastAbility());
		}

		UpdateDebuffIcons();
	}

	private IEnumerator CastAbility()
	{
		isCasting = false;
		Debug.Log("Cast Ability");
		yield return currentAbility.ActivateAbility();
		currentAbility.EndAbility();
	}

	public void CancelCasting()
	{
		isCasting = false;
		currentAbility = null;
	}

	public bool IsCasting()
	{
		return isCasting;
	}

	private void UpdateDebuffIcons()
	{
		// Удаляем старые иконки
		// foreach (Transform child in DebuffContainer)
		// {
		// 	Destroy(child.gameObject);
		// }

		// Создаем новые иконки для каждого дебаффа
		// foreach (var debuff in GetComponent<Unit>().DebuffSystem.CurrentDebuffs)
		// {
		// 	var debuffIcon = Instantiate(DebuffIconPrefab, DebuffContainer);
		// 	// Здесь вы можете установить спрайт иконки в соответствии с дебаффом
		// 	// debuffIcon.GetComponent<Image>().sprite = debuff.Icon;
		// }
	}
}