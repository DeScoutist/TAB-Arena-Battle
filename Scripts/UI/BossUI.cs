using System.Collections;
using UnitSystem;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
	public Unit Boss; // Ссылка на босса
	public Image HealthBar; // Полоска здоровья босса
	public Image CastBar; // Полоска каста босса

	private void Awake()
	{
		// Подписываемся на событие
		SpawnUnit.OnBossSpawned += HandleBossSpawned;
	}

	private void OnDestroy()
	{
		// Отписываемся от события
		SpawnUnit.OnBossSpawned -= HandleBossSpawned;
	}

	private void HandleBossSpawned(Unit boss)
	{
		// Устанавливаем ссылку на босса
		Boss = boss;

		// Подписываемся на события изменения здоровья и каста босса
		Boss.onHealthChanged += UpdateHealthBar;
		Boss.GetComponent<UnitUI>().onCastChanged += UpdateCastBar;
		
		// Подписываемся на событие
		SpawnUnit.OnBossSpawned += HandleBossSpawned;
		
		// Обновляем полоски здоровья и каста
		
		// TODO: gavno vot eto lomaet vse
		// UpdateHealthBar(Boss.HealthPercentage);
		// UpdateCastBar(0); // Замените на актуальное значение процента каста

		// Активируем полоски здоровья и каста
		HealthBar.gameObject.SetActive(true);
		CastBar.gameObject.SetActive(true);
	}
	
	private void Start()
	{
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

	private void UpdateHealthBar(float healthPercentage, Unit dealer, Unit receiver)
	{
		// Обновляем полоску здоровья босса
		HealthBar.fillAmount = healthPercentage;
	}

	private void UpdateCastBar(float castPercentage)
	{
		if (castPercentage <= 0.1f)
		{
			CastBar.gameObject.SetActive(true);
			CastBar.color = new Color(CastBar.color.r, CastBar.color.g, CastBar.color.b, 1);
		}
		
		// Обновляем полоску каста босса
		CastBar.fillAmount = castPercentage;
		
		if (castPercentage >= 1)
		{
			StartCoroutine(FadeOutCastBar());
		}
	}
}