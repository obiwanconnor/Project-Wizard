using UnityEngine;
using UnityEngine.SceneManagement;

namespace WhereAreMyKeys.UI
{
    /// <summary>
    /// Main menu entry point. Just the Play button for now (GDD has no
    /// other menu needs yet) - loads the main level by scene name.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string mainLevelSceneName = "Dungeon";

        public void PlayButtonClicked() => SceneManager.LoadScene(mainLevelSceneName);
    }
}
