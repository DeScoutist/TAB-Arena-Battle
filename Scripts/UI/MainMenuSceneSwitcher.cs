using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
	public class MainMenuSceneSwitcher : MonoBehaviour
	{
		public void SwitchToInventory()
		{
			SceneManager.LoadScene("Inventory");
		}

		public void SwitchToDungeonSelection()
		{
			SceneManager.LoadScene("GameScene");
		}

		public void QuitGame()
		{
			Application.Quit();
		}
	}
}