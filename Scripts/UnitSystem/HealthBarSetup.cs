using UnityEngine;
using UnityEngine.UI;

public class HealthBarSetup : MonoBehaviour
{
	public Image healthSlider;

	void Start()
	{
		// Настраиваем фон и заполнение
		var background = healthSlider.transform.parent.parent.Find("Background").GetComponent<Image>();
		background.color = Color.gray;
		healthSlider.color = Color.green;
	}
}