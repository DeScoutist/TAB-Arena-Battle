using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	public class DebuffSystem : MonoBehaviour
	{
		private List<GameplayTag.Authoring.GameplayTagScriptableObject> currentTags = new List<GameplayTag.Authoring.GameplayTagScriptableObject>();

		public void AddTag(GameplayTag.Authoring.GameplayTagScriptableObject tag)
		{
			if (!currentTags.Contains(tag))
			{
				currentTags.Add(tag);
			}
		}
		
		public void RemoveTag(GameplayTag.Authoring.GameplayTagScriptableObject tag)
		{
			if (currentTags.Contains(tag))
			{
				currentTags.Remove(tag);
			}
		}
		
		public bool HasTag(GameplayTag.Authoring.GameplayTagScriptableObject tag)
		{
			return currentTags.Contains(tag);
		}
	}
}