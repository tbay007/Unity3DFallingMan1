using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Advertisements;
using System.Linq;
using Assets.Scripts.Models;

public class PublicSettingsManagerScript : MonoBehaviour
{

    public static GameSettingsModel settingsModel;
    public GameManager gameManager;
    public static int Level = 1;
    public static string LevelString = "Level: " + Level;
    public static int Score = 0;
    public static string ScoreString = "Score: 0";

    public static int ScreenWidth = Screen.width;
    public static int ScreenHeight = Screen.height;

    public static Vector3 CoinSize;

#if UNITY_IOS
    private string gameId = "1752898";

#elif UNITY_ANDROID
    private string gameId = "1752899";
#endif

    public PublicSettingsManagerScript()
    {

    }

    private void Start()
    {
        try
        {
            Advertisement.Initialize(gameId, false);
#if UNITY_IOS
            Application.targetFrameRate = 60;
#endif
            using (gameManager = new GameManager())
            {
                gameManager.CreateStorageFile();
                settingsModel = gameManager.GetGameSettingsAndHighScores();
                if (settingsModel == null)
                {
                    settingsModel = new GameSettingsModel();
                    settingsModel.Exploded = false;
                    settingsModel.DateEntered = DateTime.UtcNow.ToString();
                    settingsModel.HighScores = new List<HighScoreModel>();
                    settingsModel.Id = 1;
                    settingsModel.platformSpeed = 120f;
                    settingsModel.spawnSpeed = 2f;
                    settingsModel.Volume = 1;
                    settingsModel.Died = 0;

                }
                else
                {
                    Level = 1;
                    LevelString = "Level: " + Level;
                    Score = 0;
                    ScoreString = "Score: 0";
                    settingsModel.platformSpeed = 120f;
                }

                float height = Camera.main.orthographicSize * 2;
                float minWidth = Camera.main.orthographicSize * -1;
                float maxWidth = Camera.main.orthographicSize;
                float minheight = height * Camera.main.aspect * -1;
                float maxHeight = height * Camera.main.aspect;

                float x = UnityEngine.Random.Range((float)minWidth + (float)0.64, (float)maxWidth - (float)0.64);
                float y = UnityEngine.Random.Range((float)minheight + (float)0.64, (float)maxHeight - (float)0.64);


                GameObject selfGameObject = GameObject.FindGameObjectWithTag("Coin");
                selfGameObject.transform.SetPositionAndRotation(new Vector3(x, y, 0), Quaternion.identity); // at the desired position...
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static void CheckLevel()
    {
        if ((Score % 5 == 0 && Level != 100) || Level < ((Score / 5) + 1))
        {
            if (settingsModel != null)
            {
                settingsModel.platformSpeed += 0.10f;
                settingsModel.spawnSpeed = settingsModel.spawnSpeed - (settingsModel.spawnSpeed * 0.05f);
                Level += 1;
                LevelString = "Level: " + Level.ToString();
            }
        }
    }

    private void Update()
    {
        if (settingsModel != null)
        {
            if (settingsModel.Exploded)
            {
                GameObject gameOver = GameObject.FindGameObjectWithTag("GameOver");
                if (gameOver != null)
                {
                    settingsModel.platformSpeed = 0;
                    settingsModel.spawnSpeed = 0;
                    var gameOverSprite = gameOver.GetComponent<SpriteRenderer>();
                    gameOverSprite.enabled = true;
                }
                GameObject ps = GameObject.FindGameObjectWithTag("Explosion");
                ParticleSystem particles = ps.GetComponent<ParticleSystem>();
                if (particles != null)
                {
                    if (particles.isStopped)
                    {
                        LoadBeginningLevel();
                    }
                }
            }
        }
        GameObject selfGameObject = GameObject.FindGameObjectWithTag("Coin");
        if (selfGameObject != null)
        {
            if (!settingsModel.Exploded){
                SpriteRenderer render = selfGameObject.GetComponent<SpriteRenderer>();
                if (!render.enabled)
                {
                    float height = Camera.main.orthographicSize * 2;
                    float minWidth = Camera.main.orthographicSize * -1;
                    float maxWidth = Camera.main.orthographicSize;
                    float minheight = height * Camera.main.aspect * -1;
                    float maxHeight = height * Camera.main.aspect;

                    float x = UnityEngine.Random.Range((float)minWidth + (float)0.64, (float)maxWidth - (float)0.64);
                    float y = UnityEngine.Random.Range((float)minheight + (float)0.64, (float)maxHeight - (float)0.64);

                    selfGameObject.transform.SetPositionAndRotation(new Vector3(x, y, 0), Quaternion.identity);
                    render.enabled = true;
                    CoinDisappearScript.UpdateStartTime();
                }
            }
        }
    }

    public void LoadBeginningLevel()
    {
        if (settingsModel != null)
        {
            settingsModel.platformSpeed = 2f;
            settingsModel.spawnSpeed = 2f;
            settingsModel.Exploded = false;
            settingsModel.HighScores = CalculateTop5HighScore(Score);
            settingsModel.Died += 1;
            if (settingsModel.Died == 3)
            {
                settingsModel.Died = 0;
                Advertisement.Show();
            }
            using (gameManager = new GameManager())
            {
                try
                {
                    gameManager.SaveItem(settingsModel);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        Level = 1;
        Score = 0;
        SceneManager.LoadScene(0);
    }

    public List<HighScoreModel> CalculateTop5HighScore(int NewGameScore)
    {
        List<HighScoreModel> modelList = new List<HighScoreModel>();
        using (gameManager = new GameManager())
        {
            GameSettingsModel model = gameManager.GetGameSettingsAndHighScores();
            if (model != null && model.HighScores != null && model.HighScores.Count > 0)
            {
                model.HighScores = model.HighScores.OrderByDescending(x => x.Score).ThenByDescending(y => y.ScoreDate).ToList();
                HighScoreModel newHighScore = new HighScoreModel() { Id = model.HighScores.Max(x => x.Id) + 1, Level = Level, Score = NewGameScore, ScoreDate = DateTime.UtcNow.ToString() };
                model.HighScores.Add(newHighScore);
                if (model.HighScores.Count > 5)
                {
                    model.HighScores = model.HighScores.OrderByDescending(x => x.Score).ThenByDescending(y => y.ScoreDate).ToList();
                    model.HighScores.RemoveRange(5, 1);
                }
                modelList.AddRange(model.HighScores);
            }
            else
            {
                HighScoreModel item = new HighScoreModel();
                item.Id = 1;
                item.Level = Level;
                item.Score = NewGameScore;
                item.ScoreDate = DateTime.UtcNow.ToString();
                modelList.Add(item);
            }
        }

        return modelList;
    }

}
