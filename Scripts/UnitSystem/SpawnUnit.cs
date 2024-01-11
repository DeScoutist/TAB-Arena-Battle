using UnitSystem;
using UnityEngine;

public class SpawnUnit : MonoBehaviour
{
	public GameObject tankPrefab; // Префаб танка
	public GameObject healerPrefab; // Префаб танка
	public GameObject dpsPrefab; // Префаб танка
	public GameObject bossPrefab;
	public Transform spawnPoint; // Точка спауна

	// Метод для создания экземпляра танка
	public void Spawn()
	{
		// Создаем новый экземпляр танка на точке спауна
		GameObject tank = Instantiate(tankPrefab, spawnPoint.position, spawnPoint.rotation);

		// Добавляем новый юнит в UnitUIManager
		FindObjectOfType<UnitUIManager>().AddUnit(tank.GetComponent<Unit>());

		// Создаем новый экземпляр танка на точке спауна
		GameObject healer = Instantiate(healerPrefab, spawnPoint.position, spawnPoint.rotation);

		// Добавляем новый юнит в UnitUIManager
		FindObjectOfType<UnitUIManager>().AddUnit(healer.GetComponent<Unit>());

		// Создаем новый экземпляр танка на точке спауна
		GameObject dps = Instantiate(dpsPrefab, spawnPoint.position, spawnPoint.rotation);

		// Добавляем новый юнит в UnitUIManager
		FindObjectOfType<UnitUIManager>().AddUnit(dps.GetComponent<Unit>());

		// Создаем новый экземпляр танка на точке спауна
		GameObject boss = Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);

		// Добавляем новый юнит в UnitUIManager
		FindObjectOfType<UnitUIManager>().AddUnit(boss.GetComponent<Unit>());
	}
}