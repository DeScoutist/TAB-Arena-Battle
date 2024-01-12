﻿using System.Collections;
using UnityEngine;
using UnitSystem;

public class PlayerAI : MonoBehaviour
{
    protected const float WAIT_ATTACK_DURATION = 2f;
    protected const float LINE_RENDERER_WIDTH = 0.1f;
    protected const string BOSS_TAG = "BOSS_TAG";
    protected const string DEFAULT_SHADER = "Sprites/Default";

    [SerializeField]
    protected float speed = 5f;
    [SerializeField]
    protected float attackRadius = 4f;
    [SerializeField]
    protected float attackDamage = 10f;
    [SerializeField]
    protected float stopDistance = 3f;

    protected GameObject target;
    protected Coroutine attackRoutine;
    protected bool isTaskedToRun = false;
    protected Vector3 positionToRunTo;
    protected LineRenderer lineRenderer;
    protected Unit targetEnemy;
    protected bool isInCombat = false;

    protected virtual void Start()
    {
        target = GameObject.FindGameObjectWithTag(BOSS_TAG);
        InitializeLineRenderer();
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
        ManageMovement();

        if(IsPlayerControlled())
        {
            HandlePlayerInput();
            lineRenderer.enabled = true;
        }
        else
        {
            HandleAITasks();
        }

        if (isInCombat)
        {
            if (attackRoutine == null)
            {
                attackRoutine = StartCoroutine(AttackTarget());
            }
        }
    }

    protected bool IsPlayerControlled()
    {
        return UnitSelection.selectedUnit == this.GetComponent<Unit>();
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
        bool isHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        Unit hitUnit = isHit ? hitInfo.transform.GetComponent<Unit>() : null;

        if (hitUnit != null)
        {
            MoveOrAttack(hitInfo);
        }
        else
        {
            MoveToPoint();
        }
    }

    protected virtual void MoveOrAttack(RaycastHit unitHit)
    {
        Unit enemy = unitHit.transform.GetComponent<Unit>();
        if (enemy != null && enemy.isEnemy)
        {
            targetEnemy = enemy;
            isInCombat = true;
            if (TargetIsOutOfAttackRange())
            {
                MoveAgainstTarget();
            }
        }
    }

    protected void MoveToPoint()
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);
        targetEnemy = null;

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
            if (targetEnemy)
            {
                positionToRunTo = targetEnemy.transform.position;
            }

            transform.position = Vector3.MoveTowards(transform.position, positionToRunTo, speed * Time.deltaTime);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, positionToRunTo);    // Set the end position of the line
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
        return targetEnemy != null && Vector3.Distance(transform.position, targetEnemy.transform.position) <= attackRadius;
    }

    protected IEnumerator AttackTarget()
    {
        while (target != null && Vector3.Distance(transform.position, target.transform.position) <= attackRadius)
        {
            target.GetComponent<Unit>().ChangeHealth(-attackDamage);
            yield return new WaitForSeconds(WAIT_ATTACK_DURATION);
        }

        attackRoutine = null;
    }

    protected bool TargetIsOutOfAttackRange()
    {
        return Vector3.Distance(transform.position, targetEnemy.transform.position) > attackRadius;
    }

    protected void MoveAgainstTarget()
    {
        positionToRunTo = targetEnemy.transform.position;
        isTaskedToRun = true;
    }
}