using UnityEngine;

[CreateAssetMenu(fileName = "New Debuff", menuName = "Debuff")]
public class DebuffEffect : ScriptableObject
{
	public float Duration; // Продолжительность дебаффа
	public float TickInterval; // Интервал срабатывания дебаффа
	public float TickDamage; // Урон от каждого срабатывания
}