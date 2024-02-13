using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitFrameController : MonoBehaviour
{
	public GameObject SelectedUnitFrame;
	public GameObject TargetedUnitFrame;

	public Image SelectedUnitIcon;
	public TMP_Text SelectedUnitName;
	public Image SelectedUnitHealthBar;
	public Image SelectedUnitCastBar;

	public Image TargetedUnitIcon;
	public TMP_Text TargetedUnitName;
	public Image TargetedUnitHealthBar;
	public Image TargetedUnitCastBar;

	private void Start()
	{
		SelectedUnitFrame.SetActive(false);
		TargetedUnitFrame.SetActive(false);
	}
}