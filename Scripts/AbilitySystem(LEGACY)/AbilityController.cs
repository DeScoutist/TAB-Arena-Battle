// using System;
// using System.Collections;
// using System.Collections.Generic;
// using AbilitySystem;
// using UnitSystem;
// using UnityEngine;
//
// public class AbilityController : MonoBehaviour
// {
// 	public List<Ability> activeAbilities = new List<Ability>(); // Список активных способностей
// 	private Unit _unit;
//
// 	private void Start()
// 	{
// 		_unit = GetComponent<Unit>();
// 	}
//
// 	public void ActivateAbility(Ability ability, Vector3 position, Quaternion rotation)
// 	{
// 		// Добавляем способность в список активных
// 		activeAbilities.Add(ability);
//
// 		// Вызываем метод активации способности
// 		ability.Activate(position, rotation);
//
// 		// Запускаем корутину
// 		StartCoroutine(StartCasting(ability));
// 	}
//
// 	private IEnumerator StartCasting(Ability ability)
// 	{
// 		_unit.isCasting = true;
//
// 		yield return new WaitForSeconds(ability.castTime);
//
// 		if (ability.isVoidZone)
// 		{
// 			StartCoroutine(ability.DealDamageOverTime());
// 		}
//
// 		// Вызываем метод выполнения эффекта способности
// 		// ability.PerformEffect();
//
// 		// Удаляем способность из списка активных
// 		_unit.GetComponent<BossAI>().currentAbility = null;
// 		_unit.isCasting = false;
// 		activeAbilities.Remove(ability);
// 	}
// }