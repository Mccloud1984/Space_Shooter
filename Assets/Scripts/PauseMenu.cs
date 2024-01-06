using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class PauseMenu : MonoBehaviour
{
    

    public void ResumePlay()
    {
        GameConfig.GetGameManager().ResumePlay();
    }
    public void BackToMainMenu()
    {
        GameConfig.GetGameManager().ToMainMenu();
    }
}
