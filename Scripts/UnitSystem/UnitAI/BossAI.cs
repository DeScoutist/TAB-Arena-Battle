﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem;
 using AbilitySystem.Authoring;
 using UnitSystem;
using UnityEngine;
 using UnityEngine.Serialization;
 using Random = UnityEngine.Random;

public class BossAI : MonoBehaviour
{
    public float speed = 5f;
    public float attackRadius = 4f;
    public float attackDamage = 0f;
    private AbilityController _controller;
    public Unit Target;
    private Unit meUnit;
    private Coroutine attackRoutine;
    public AbstractAbilitySpec currentAbility;
    public float stopDistance = 3f;
    
    [SerializeField]
    private AbilityController abilityController;

    private void Start()
    {
        meUnit = GetComponent<Unit>();
        _controller = GetComponent<AbilityController>();
    }

    void Update()
    {
        Unit closestEnemy = FindClosestEnemy();

        if (closestEnemy == null)
            return;

        float distance = Vector3.Distance(transform.position, closestEnemy.transform.position);

        bool canMoveAndRotate = currentAbility == null || !(currentAbility.isCasting && meUnit.isCasting);

        if (canMoveAndRotate)
        {
            MoveTowardsEnemy(closestEnemy, distance);
            RotateTowardsEnemy(closestEnemy);
        }

        if (distance <= attackRadius)
            InitiateAttackOnEnemy(closestEnemy);

        // TODO: Нужно добавить функциональность радиусов способностей.
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
        
        // Получаем список способностей, которые не на кулдауне
        var availableAbilities = _controller.abilitySpecs.Where(a => a.CheckCooldown().TimeRemaining <= 0).ToList();

        if (!availableAbilities.Any())
            return;

        // Выбираем случайную способность из списка доступных
        // AbstractAbilitySpec chosenAbility = availableAbilities[UnityEngine.Random.Range(0, availableAbilities.Count)];

        // Активируем выбранную способность
        this.abilityController.UseAbility(UnityEngine.Random.Range(0, availableAbilities.Count));
        // StartCoroutine(chosenAbility.TryActivateAbility());
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
        while (Target != null && Vector3.Distance(transform.position, Target.transform.position) <= attackRadius)
        {
            Target.ChangeHealth(-attackDamage);
            yield return new WaitForSeconds(2f);
        }
    }
}