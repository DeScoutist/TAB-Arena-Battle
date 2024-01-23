// using UnitSystem;
// using UnityEngine;
//
// public class FireballController : MonoBehaviour
// {
// 	public float speed = 10f; // Speed of the fireball
// 	public float damageAmount; // Damage dealt by the fireball
// 	public bool penetratePlayers; // Whether the fireball should penetrate players
//
// 	void Update()
// 	{
// 		// Move the fireball forward
// 		transform.position += transform.forward * speed * Time.deltaTime;
// 	}
//
// 	void OnTriggerEnter(Collider other)
// 	{
// 		// If the fireball hits a player
// 		if (other.CompareTag("Player"))
// 		{
// 			// Deal damage to the player
// 			other.GetComponent<Unit>().ChangeHealth(-damageAmount);
//
// 			// If the fireball should not penetrate players, destroy it
// 			if (!penetratePlayers)
// 			{
// 				Destroy(gameObject);
// 			}
// 		}
// 	}
// }