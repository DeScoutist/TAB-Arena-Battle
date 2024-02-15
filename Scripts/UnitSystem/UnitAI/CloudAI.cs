using System.Collections;
using UnitSystem.UnitAI;
using UnityEngine;

public class CloudAI : MonoBehaviour, IAI
{
	public float speed = 1f;
	public float directionChangeInterval = 7f;
	private Vector3 movementDirection;

	public UnityEngine.Transform transform => base.transform;
	public Vector3 SpellTargetPosition { get; set; }
	public Quaternion SpellTargetRotation { get; set; }
	
	void Start()
	{
		StartCoroutine(ChangeDirection());
	}

	void Update()
	{
		transform.position += movementDirection * speed * Time.deltaTime;
	}

	IEnumerator ChangeDirection()
	{
		while (true)
		{
			// Случайное направление движения
			movementDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
			yield return new WaitForSeconds(directionChangeInterval);
		}
	}
}