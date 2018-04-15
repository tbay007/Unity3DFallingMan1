using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {
    void OnMouseDown()
    {
        SceneManager.LoadScene(1);
    }
}
