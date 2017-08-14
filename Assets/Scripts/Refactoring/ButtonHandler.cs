using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour {

    public void ResumeGame(GameObject prefab)
    {
        UIManager.Instance.DestroyPopup(prefab);
    }

    public void RestartGame()
    {
        GameManager2.Instance.TriggerReset();
    }

    public void MainMenu()
    {
        GameManager2.Instance.LoadMainMenu();
    }

    public void NextLevel()
    {
        GameManager2.Instance.LoadNextLevel();
    }

    public void RemovePopup(GameObject prefab)
    {
        UIManager.Instance.DestroyPopup(prefab);
    }

}
