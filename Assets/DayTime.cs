using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.Experimental.GlobalIllumination;

public class DayTime : MonoBehaviour
{
    MeshRenderer mr;
    float val = 0;
    public Text time;
    public Text day;
    public Text debugPanel;
    private float offsetSkybox = 0.0833f;
    private float offsetLightIntensive = 0.1f;

    [HideInInspector]
    public DateTime dt;
    int passed;

    void Start()
    {
       // mr = GetComponent<MeshRenderer>();
        dt = DateTime.Now;

        int oldPassed = PlayerPrefs.GetInt("OldTimePassed", 0);
        TimeSpan differenceOldQuit = dt - DateTime.Parse(PlayerPrefs.GetString("LastAppClosed", dt.ToString()));

        passed = (int)differenceOldQuit.TotalMinutes + oldPassed;
        debugPanel.text = String.Format("{0} | {1}\nTime {2} | Buffer: {3} min",
            PlayerPrefs.GetString("LastAppClosed", DateTime.Now.ToString()),DateTime.Now, PlayerPrefs.GetString("LastAction","NoAction"),passed);
  
        int addHours = passed / 60;
        UpdateTimeMatrix(addHours);
        passed = passed % 60;

        val = offsetSkybox * dt.Hour;
        //mr.material.mainTextureOffset = new Vector2(val, 0f);

        day.text = dt.DayOfWeek.ToString();
        StartCoroutine(TimeControl());
    }

    void TimeUpdate()
    {
        dt = DateTime.Now;

        //переход на следующий день
        if (dt.Hour == 0)
        {
            day.text = dt.DayOfWeek.ToString();
            val = 0;
        }

        //добавление часа времени
        if (dt.Minute == 0)
        {
            val += offsetSkybox;
           // mr.material.mainTextureOffset = new Vector2(val, 0f);
        }
        time.text = dt.ToString("HH:mm");
    }

    //вызов проверки времени раз в минуту
    IEnumerator TimeControl()
    {
        for (; ;)
        {
            if (passed == 60)
            {
                passed = 0;
                UpdateTimeMatrix(1);
            }
            else
                passed++;
             
            Debug.Log(String.Format("Буфер минут: {0}", passed));
            TimeUpdate();
            yield return new WaitForSeconds(60f);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            PlayerPrefs.SetString("LastAppClosed", dt.ToString());
            PlayerPrefs.SetInt("OldTimePassed", passed);
            PlayerPrefs.Save();
        }
        Debug.Log(DateTime.Now+" App focus "+focus);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerPrefs.SetString("LastAppClosed", dt.ToString());
            PlayerPrefs.SetInt("OldTimePassed", passed);
            PlayerPrefs.Save();
        }
        Debug.Log(DateTime.Now + " App pause " + pause);
    }

    public void UpdateTimeMatrix(int hours)
    {
        Debug.Log("Матрица времени обновлена на "+hours+" час(ов)");
    }
}
