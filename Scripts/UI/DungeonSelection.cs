using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class DungeonSelection : MonoBehaviour
	{
		// [SerializeField] private List<Dungeon> dungeons; // список подземелий
		[SerializeField] private List<int> dungeons; // список подземелий
		[SerializeField] private GameObject dungeonButtonPrefab; // префаб кнопки подземелья
		[SerializeField] private Transform dungeonList; // родительский элемент для кнопок подземелья
		[SerializeField] private Text dungeonDescription; // текстовое поле для описания подземелья
		[SerializeField] private Image dungeonImage; // изображение подземелья
		[SerializeField] private Text availableHeroes; // текстовое поле для доступных героев
		[SerializeField] private Text rewards; // текстовое поле для наград

		private void Start()
		{
			for (int i = 0; i < 4; i++)
			{
				dungeons.Add(i);
			}
		
			foreach (var dungeon in dungeons)
			{
				var button = Instantiate(dungeonButtonPrefab, dungeonList).GetComponent<Button>();
				// button.GetComponentInChildren<Text>().text = dungeon.Name;
				button.GetComponentInChildren<Text>().text = dungeon.ToString();
				// button.onClick.AddListener(() => SelectDungeon(dungeon));
			}
		}

		private void SelectDungeon(Dungeon dungeon)
		{
			dungeonDescription.text = dungeon.Description;
			dungeonImage.sprite = dungeon.Image;
			availableHeroes.text = string.Join(", ", dungeon.AvailableHeroes);
			rewards.text = string.Join(", ", dungeon.Rewards);

			// увеличиваем изображение
			dungeonImage.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
		}
	}
}

