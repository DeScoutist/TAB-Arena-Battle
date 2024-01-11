using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	public float speed = 0.1f; // Скорость перемещения камеры
	private Vector3 dragOrigin; // Точка, где началось перетаскивание

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			// Запоминаем точку, где началось перетаскивание
			dragOrigin = Input.mousePosition;
		}

		if (Input.GetMouseButton(0))
		{
			// Вычисляем перемещение мыши
			Vector3 difference = Input.mousePosition - dragOrigin;
			dragOrigin = Input.mousePosition;

			// Если мышь переместилась, перемещаем камеру
			if (difference != Vector3.zero)
			{
				Vector3 pos = Camera.main.ScreenToViewportPoint(difference);
				Vector3 move = new Vector3(-pos.x * speed, 0, -pos.y * speed);

				// Перемещаем камеру
				transform.Translate(move, Space.World);
			}
		}
	}
}