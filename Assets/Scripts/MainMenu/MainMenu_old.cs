using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    public int _defaultGameScene = 1; //Game Scene
    public void LoadGame()
    {
        SceneManager.LoadScene(_defaultGameScene);
    }
}
