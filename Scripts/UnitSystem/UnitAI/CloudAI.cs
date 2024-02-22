using System.Collections;
using UnitSystem.UnitAI;
using Unity.VisualScripting;
using UnityEngine;
using Unit = UnitSystem.Unit;

public class CloudAI : MonoBehaviour, IAI
{
	public float speed = 1f;
	public float directionChangeInterval = 7f;
	private Vector3 movementDirection;

	public UnityEngine.Transform transform => base.transform;
	public Vector3 SpellTargetPosition { get; set; }
	public Quaternion SpellTargetRotation { get; set; }

	private Unit thisUnit;

	void Start()
	{
		thisUnit = GetComponent<Unit>();
		StartCoroutine(ChangeDirection());
	}

	void Update()
	{
		if (thisUnit.isDead)
		{
			return;
		}

		transform.position += movementDirection * speed * Time.deltaTime;
	}

	IEnumerator ChangeDirection()
	{
		while (true)
		{
			if (thisUnit.isDead)
			{
				yield break;
			}
			// Случайное направление движения
			movementDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
			yield return new WaitForSeconds(directionChangeInterval);
		}
	}
}