using UnityEngine;

public class MoveOnClick3D : MonoBehaviour
{
	public float moveSpeed = 5f; // скорость перемещения объекта
	private Vector3 targetPos;

	private void Start()
	{
		targetPos = transform.position; // начальная позиция объекта
	}

	private void Update()
	{
		// // Проверяем, был ли совершён клик левой кнопкой мыши
		// if (Input.GetMouseButtonDown(0))
		// {
		// 	RaycastHit hit;
		//           
		// 	// Отправляем луч от позиции камеры в направлении мыши
		// 	if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
		// 	{
		// 		targetPos = hit.point; // если встретили коллайдер, сохраняем точку пересечения как цель
		// 		targetPos.y = transform.position.y; // сохраняем первоначальную высоту, чтобы объект не двигался по вертикали
		// 	}
		// }
		//
		// // Плавное перемещение объекта к указанной позиции
		// transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
	}
}