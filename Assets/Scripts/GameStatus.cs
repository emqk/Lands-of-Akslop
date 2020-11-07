using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using System.Collections;

public static class GameStatus
{
    public static void CheckGameStatus()
    {
        if (PlotCity.instance.MyCountry != null)
        {
            if (!CountryManager.instance.IsItDefaultCountry(PlotCity.instance.MyCountry))
            {
                if(PlotCity.instance.MyCountry.isPlayerCountry)
                    Debug.Log("Game end, player won, reason: PlotCity conquered");
                else
                    Debug.Log("Game end, player lost, reason: PlotCity conquered");

                OnGameEnd();
                return;
            }
        }

        if (CountryManager.instance.PlayerCountry.GetCitiesCount() <= 0)
        {
            Debug.Log("Game end, player lost, reason: All player's cities conquered by AI");
            OnGameEnd();
            return;
        }

        if (CountryManager.instance.IsEveryCityDefaultOrSameCountry())
        {
            Debug.Log("Game end, player won, reason: All other countries cities conquered");
            OnGameEnd();
            return;
        }
    }

    static void OnGameEnd()
    {
        SceneManager.LoadScene("MainMenu");
    }
}