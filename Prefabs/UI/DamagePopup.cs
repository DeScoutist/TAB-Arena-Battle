using System;
using System.Collections;
using System.Collections.Generic;
using Prefabs.UI;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
	private TextMeshPro textMesh;
	
	[SerializeField] private Transform pfDamagePopup;
	
	public static DamagePopup Create(Vector3 position, int damageAmount)
	{
		Transform PopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);
		var damagePopup = PopupTransform.GetComponent<DamagePopup>();
		damagePopup.Setup(damageAmount);

		return damagePopup;
	}
	
	private void Awake()
	{
		textMesh = transform.GetComponent<TextMeshPro>();
	}
	
	public void Setup(int damageAmount)
	{
		textMesh.SetText(damageAmount.ToString());
	}
}
