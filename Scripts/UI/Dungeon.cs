using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	[CreateAssetMenu(fileName = "New Dungeon", menuName = "Dungeon")]
	public class Dungeon : ScriptableObject
	{
		public string Name;
		public string Description;
		public Sprite Image;
		public List<string> AvailableHeroes;
		public List<string> Rewards;
	}
}