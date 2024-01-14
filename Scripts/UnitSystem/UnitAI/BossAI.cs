﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem;
using UnitSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossAI : MonoBehaviour
{
    public float speed = 5f;
    public float attackRadius = 4f;
    public float attackDamage = 10f;
    public List<Ability> abilities;
    private Dictionary<Ability, float> abilityLastUsedTimes = new Dictionary<Ability, float>();
    public AbilityController _controller;
    private Unit target;
    private Unit meUnit;
    private Coroutine attackRoutine;
    public Ability currentAbility;
    public float stopDistance = 3f;

    private void Start()
    {
        meUnit = GetComponent<Unit>();
        foreach (var ability in abilities)
        {
            abilityLastUsedTimes[ability] = -ability.cooldown;
        }
    }

    void Update()
    {
        Unit closestEnemy = FindClosestEnemy();

        if (closestEnemy == null)
            return;

        float distance = Vector3.Distance(transform.position, closestEnemy.transform.position);

        bool canMoveAndRotate = currentAbility == null || !(currentAbility.canBeRotatedCasting && meUnit.isCasting);

        if (canMoveAndRotate)
        {
            MoveTowardsEnemy(closestEnemy, distance);
            RotateTowardsEnemy(closestEnemy);
        }

        if (distance <= attackRadius)
            InitiateAttackOnEnemy(closestEnemy);

        if (currentAbility == null && !meUnit.isCasting)
            ActivateRandomAbility();
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

    private void ActivateRandomAbility()
    {
        // Получаем список способностей, которые не на кулдауне
        List<Ability> availableAbilities = abilities.Where(a =>
            abilityLastUsedTimes.ContainsKey(a) &&
            Time.time - abilityLastUsedTimes[a] >= a.cooldown).ToList();

        if (!availableAbilities.Any())
            return;

        // Выбираем случайную способность из списка доступных
        Ability chosenAbility = availableAbilities[UnityEngine.Random.Range(0, availableAbilities.Count)];

        // Update the last used time
        abilityLastUsedTimes[chosenAbility] = Time.time;

        currentAbility = chosenAbility;

        Vector3 direction = target.transform.position - transform.position;
        Quaternion rotation = Quaternion.Euler(0, -Quaternion.LookRotation(direction).eulerAngles.y, 0);

        Ability newAbility = Instantiate(currentAbility);
        _controller.ActivateAbility(newAbility, transform.position, rotation);
        StartCoroutine(WaitForAbilityToFinish(newAbility));
    }

    private IEnumerator WaitForAbilityToFinish(Ability ability)
    {
        yield return new WaitForSeconds(ability.cooldown);
    }

    private Unit FindClosestEnemy()
    {
        Unit[] units = FindObjectsOfType<Unit>();
        Unit closestTank = null;
        Unit closest = null;
        Unit thisUnit = GetComponent<Unit>();
        float minDistance = Mathf.Infinity;

        foreach (Unit unit in units)
        {
            if (unit.isEnemy != thisUnit.isEnemy)
            {
                float distance = Vector3.Distance(transform.position, unit.transform.position);

                if ((closestTank == null || distance < Vector3.Distance(transform.position, closestTank.transform.position)))
                {
                    closestTank = unit;
                }

                if (distance < minDistance)
                {
                    closest = unit;
                    minDistance = distance;
                }
            }
        }

        return closestTank != null ? closestTank : closest;
    }

    private IEnumerator AttackTarget()
    {
        while (target != null && Vector3.Distance(transform.position, target.transform.position) <= attackRadius)
        {
            target.ChangeHealth(-attackDamage);
            yield return new WaitForSeconds(2f);
        }
    }
}