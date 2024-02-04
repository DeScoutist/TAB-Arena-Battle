using System.Collections.Generic;
using System.Linq;
using UnitSystem;
using UnityEngine;

public class HealerAI : PlayerAI
{
    private const float HEAL_AMOUNT = 20f;
    private Unit healTarget;
    private SpawnUnit spawnUnit;
    private float manaPool = 100f; // Манапул
    private bool isMoving = false; // Флаг, указывающий, перемещается ли юнит

    protected override void Start()
    {
        base.Start();
        
        spawnUnit = FindObjectOfType<SpawnUnit>();
    }

    protected override void Update()
    {
        base.Update();

        if (!isMoving)
        {
            SelectHealTarget();
            Dispel();
        }

        if (manaPool <= 0)
        {
            AttackWhenManaPoolIsDepleted();
        }
    }

    private List<Unit> GetGroupMembers()
    {
        return spawnUnit.units;
    }

    private void SelectHealTarget()
    {
        List<Unit> groupMembers = GetGroupMembers();

        Unit mostInjuredUnit = groupMembers.FirstOrDefault(unit => unit.HealthPercentage <= 20);
        if (mostInjuredUnit != null)
        {
            healTarget = mostInjuredUnit;
            HealUnit(healTarget);
            return;
        }

        Unit tank = groupMembers.FirstOrDefault(unit => unit.isTank && unit.HealthPercentage <= 70);
        if (tank != null)
        {
            healTarget = tank;
            HealUnit(healTarget);
            return;
        }

        Unit injuredUnit = groupMembers.FirstOrDefault(unit => unit.HealthPercentage < 100);
        if (injuredUnit != null)
        {
            healTarget = injuredUnit;
            HealUnit(healTarget);
            return;
        }

        healTarget = null;
    }

    private void Dispel()
    {
        // Реализация снятия заклинания с союзника каждые 4 секунды
        // Это псевдокод, вам нужно добавить реальную реализацию
    }

    private void AttackWhenManaPoolIsDepleted()
    {
        // Реализация атаки, когда манапул исчерпан
        // Это псевдокод, вам нужно добавить реальную реализацию
    }

    protected override void MoveToPoint()
    {
        base.MoveToPoint();
        isMoving = true;
    }

    protected override void HandleAITasks()
    {
        base.HandleAITasks();
        isMoving = false;
    }

    private void HealUnit(Unit unit)
    {
        // unit.ChangeHealth(HEAL_AMOUNT);
        manaPool -= HEAL_AMOUNT; // Уменьшаем манапул на количество затраченного на лечение маны
    }
}