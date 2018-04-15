using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinDisappearScript : MonoBehaviour
{

    public int Duration = 3;
    public static DateTime StartTime;
    private float rotSpeed = 60f;
    void Start()
    {
        StartTime = DateTime.Now;
    }

    public static void UpdateStartTime()
    {
        StartTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        DateTime updateDate = DateTime.Now;
        TimeSpan ts = updateDate - StartTime;
        if (ts.Seconds >= Duration)
        {
            GameObject selfGameObject = GameObject.FindGameObjectWithTag("Coin");
            if (selfGameObject != null)
            {
                SpriteRenderer render = selfGameObject.GetComponent<SpriteRenderer>();
                render.enabled = false;
            }
        }
        else
        {
            GameObject selfGameObject = GameObject.FindGameObjectWithTag("Coin");
            selfGameObject.transform.Rotate(0, rotSpeed * Time.deltaTime, 0, Space.World);
        }
    }
}
