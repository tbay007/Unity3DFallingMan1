using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitHighScore : MonoBehaviour {
    void Update()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || (Input.GetMouseButtonDown(0))))
        {
            SceneManager.LoadScene(0);
        }
    }
}
