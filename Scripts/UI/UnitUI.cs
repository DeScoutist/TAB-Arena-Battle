using System.Collections;
using AbilitySystem.Authoring;
using TMPro;
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
	public Image CastBar; // Полоска каста
	// public GameObject DebuffIconPrefab; // Префаб иконки дебаффа
	// public Transform DebuffContainer; // Контейнер для иконок дебаффов

	// Define a delegate that represents the signature of the methods that can be subscribed to the event
	public delegate void CastChanged(float castPercentage);

	// Define the event
	public event CastChanged onCastChanged;
	
	private void Start()
	{
		// Получаем ссылку на персонажа
		unit = GetComponent<Unit>();

		// Подписываемся на событие изменения здоровья
		unit.onHealthChanged += UpdateHealthBar;
	}

	private void UpdateHealthBar(float healthPercentage)
	{
		// Устанавливаем заполненность полоски здоровья
		HealthBar.fillAmount = healthPercentage;
	}
	
	private void UpdateCastBar(float castPercentage)
	{
		// Устанавливаем заполненность полоски каста
		CastBar.fillAmount = castPercentage;
	}
	
	public void StartCasting(AbstractAbilitySpec ability)
	{
		currentAbility = ability;
		castStartTime = Time.time;
		isCasting = true;
		
		// Показываем индикатор каста и устанавливаем текст
		CastBar.gameObject.SetActive(true);
		CastBar.color = new Color(CastBar.color.r, CastBar.color.g, CastBar.color.b, 1);
	}

	public void Update()
	{
		if (isCasting)
		{
			// Обновляем индикатор каста
			CastBar.fillAmount = (Time.time - castStartTime) / currentAbility.Ability.CastTime;
			onCastChanged?.Invoke(CastBar.fillAmount);

			if (Time.time - castStartTime >= currentAbility.Ability.CastTime)
			{
				StartCoroutine(CastAbility());
			}
		}

		UpdateDebuffIcons();
	}

	private IEnumerator CastAbility()
	{
		isCasting = false;
		yield return currentAbility.ActivateAbility();
		currentAbility.EndAbility();

		// Проверяем, все ли еще идет каст перед началом затухания
		if (!isCasting)
		{
			StartCoroutine(FadeOutCastBar());
		}
	}

	private IEnumerator FadeOutCastBar()
	{
		// Затухание индикатора за 1 секунду
		for (float t = 0; t < 1; t += Time.deltaTime)
		{
			CastBar.color = new Color(CastBar.color.r, CastBar.color.g, CastBar.color.b, 1 - t);
			yield return null;
		}

		// Скрываем индикатор после затухания
		CastBar.gameObject.SetActive(false);
	}
	
	public void CancelCasting()
	{
		isCasting = false;
		currentAbility.EndAbility();
		currentAbility = null;

		// Скрываем индикатор при отмене каста
		CastBar.gameObject.SetActive(false);
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