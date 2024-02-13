using System.Collections.Generic;
using UnitSystem;
using UnityEngine;

public class SpawnUnit : MonoBehaviour
{
	public GameObject tankPrefab; // Префаб танка
	public GameObject healerPrefab; // Префаб танка
	public GameObject dpsPrefab; // Префаб танка
	public GameObject bossPrefab;
	public GameObject skeletonPrefab;
	public Transform spawnPoint; // Точка спауна
	public List<Unit> units = new List<Unit>(); // Список юнитов
	
	// Определите делегат и событие
	public delegate void BossSpawned(Unit boss);
	public static event BossSpawned OnBossSpawned;


	// Метод для создания экземпляра танка
	public void Spawn()
	{
		// Создаем новый экземпляр танка на точке спауна
		GameObject tank = Instantiate(tankPrefab, spawnPoint.position + new Vector3(3, 0, 2), spawnPoint.rotation);

		// Добавляем новый юнит в UnitUIManager
		// FindObjectOfType<UnitUIManager>().AddUnit(tank.GetComponent<Unit>());

		// Создаем новый экземпляр танка на точке спауна
		GameObject healer = Instantiate(healerPrefab, spawnPoint.position+ new Vector3(1, 0, 0), spawnPoint.rotation);

		// Добавляем новый юнит в UnitUIManager
		// FindObjectOfType<UnitUIManager>().AddUnit(healer.GetComponent<Unit>());

		// Создаем новый экземпляр танка на точке спауна
		GameObject dps = Instantiate(dpsPrefab, spawnPoint.position+ new Vector3(5, 0, 0), spawnPoint.rotation);

		// Добавляем новый юнит в UnitUIManager
		// FindObjectOfType<UnitUIManager>().AddUnit(dps.GetComponent<Unit>());

		// Создаем новый экземпляр танка на точке спауна
		GameObject boss = Instantiate(bossPrefab, spawnPoint.position+ new Vector3(3, 0, 20), spawnPoint.rotation);
		
		// GameObject skeleton = Instantiate(skeletonPrefab, spawnPoint.position+ new Vector3(3, 0, 23), spawnPoint.rotation);

		// Вызываем событие
		OnBossSpawned?.Invoke(boss.GetComponent<Unit>());

		units.Add(tank.GetComponent<Unit>());
		units.Add(healer.GetComponent<Unit>());
		units.Add(dps.GetComponent<Unit>());
		// Добавляем новый юнит в UnitUIManager
		// FindObjectOfType<UnitUIManager>().AddUnit(boss.GetComponent<Unit>());
	}
}