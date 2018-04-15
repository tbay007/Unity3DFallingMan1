using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPosition : MonoBehaviour
{
    private bool flag = false;
    //destination point
    private Vector3 endPoint;
    //alter this to change the speed of the movement of player / gameobject
    public float duration = 240.0f;
    //vertical position of the gameobject
    public bool collidedWithPlatform = false;
    // Use this for initialization
    public GameObject explosion;
    void Start()
    {
    }
 
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("FloorShortBarrierTransparent") || collision.name.Equals("FloorLongBarrierTransparent"))
        {
            //game over!!!
            Instantiate(explosion, transform.position, transform.rotation);
            if (PublicSettingsManagerScript.settingsModel != null)
            {
                PublicSettingsManagerScript.settingsModel.Exploded = true;
            }
            Destroy(this.gameObject);
        }
        else if (collision.name.Equals("Coin64by64Transparent"))
        {
            GameObject selfGameObject = GameObject.FindGameObjectWithTag("Coin");
            if (selfGameObject != null)
            {
                SpriteRenderer render = selfGameObject.GetComponent<SpriteRenderer>();
                render.enabled = false;

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

                PublicSettingsManagerScript.Score += 2;
                PublicSettingsManagerScript.ScoreString = "Score: " + PublicSettingsManagerScript.Score.ToString();
                PublicSettingsManagerScript.CheckLevel();
            }
        }
    }

 

    IEnumerator waitForExplosion()
    {
        yield return new WaitForSecondsRealtime(5);
   
    }

    // Update is called once per frame
    void Update()
    {
        
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || (Input.GetMouseButtonDown(0))))
            {
                RaycastHit hit;
                //Create a Ray on the tapped / clicked position
                Ray ray;
                //for unity editor
#if UNITY_EDITOR
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //for touch device
#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif

                if (Physics.Raycast(ray, out hit))
                {
                    //set a flag to indicate to move the gameobject
                    flag = true;
                    //save the click / tap position
                    endPoint = hit.point;
                    //as we do not want to change the y axis value based on touch position, reset it to original y axis value
                    //endPoint.y = yAxis;
                }

            }
            //check if the flag for movement is true and the current gameobject position is not same as the clicked / tapped position
            if (flag && !Mathf.Approximately(gameObject.transform.position.magnitude, endPoint.magnitude))
            { //&& !(V3Equal(transform.position, endPoint))){
              //move the gameobject to the desired position

            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, endPoint, 1 / ( 60 *Time.deltaTime * duration * (Vector3.Distance(gameObject.transform.position, endPoint))));
            }
            //set the movement indicator flag to false if the endPoint and current gameobject position are equal
            else if (flag && Mathf.Approximately(gameObject.transform.position.magnitude, endPoint.magnitude))
            {
                flag = false;
            }
        
        
    }

}

