using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public enum Levelstate
{
    Wait,
    Load,
    Edit,
    Play,
    Completed
}


public class LevelManager : MonoBehaviour
{

    
    [SerializeField] Levelstate levelstate  = Levelstate.Wait;
    [SerializeField] bool isplay = false;
    bool levelcompaleted = false;
    
    [SerializeField] BallMovemant ballmovemant = null;
    [SerializeField] int oldstars = 0;
    [SerializeField] int newstars = 0;
    [SerializeField] int leveltry = 0;
    [SerializeField] int maxleveltry =0;
    [SerializeField] int maxlevelbounce = 0;
    [SerializeField] string levelkey = "none";
    [SerializeField] AudioManager audioManager = null;
    [Header("load configaration")]
    [SerializeField] int curentlevel = 0;
    [SerializeField] bool islevelload = false;
    [SerializeField] GameStateManager gamestate = null;
    [SerializeField] InputManager inputManager;
    [SerializeField] List<GameObject> staticobject = null;
    [SerializeField] List<GameObject> props = null;
    [SerializeField] List<Propdata> propdata = null;
    [SerializeField] Transform ballstartpoint = null;
    [SerializeField] Transform levelendpont  =  null;


    [Header("proplist")]
    [SerializeField] GameObject normal = null;

    [Header("static object list")]
    [SerializeField] GameObject wall = null;

    private void Start()
    {

        levelstate = Levelstate.Wait;
        ballmovemant.resetBallMovemant(ballstartpoint.position);
    }

    private void Update()
    {
        chakelevelcompaleted();
        //statemanager();
    }


    //wait state
    public void unloadlevel()
    {
        levelstate = Levelstate.Wait;
        inputManager.ResetSelection();
        levelcompaleted = false;
        ballmovemant.resetBallMovemant(ballstartpoint.position);
        propdata.Clear();
        if (islevelload)
        {
          foreach (var obj in staticobject)
          {
            Destroy(obj);
          }
          foreach (var prop in props)
          {
            Destroy(prop);
          }
          props.Clear();
          staticobject.Clear();
          islevelload = false;
          isplay = false;  
          
        }
    }

    




    // play and edit state
    public void Chakeplay()
    {
        if (!isplay)
        {
            isplay = true;
            
            levelstate = Levelstate.Play;
            
        }
        else
        {
            isplay = false;
           
            levelstate = Levelstate.Edit;
            
        }

    }



 
    public void playbutton() 
    {
        if (!islevelload || levelstate == Levelstate.Completed)
        {
            return;
        }

        if (isplay && !ballmovemant.getishitend())
        {
           isplay = false;
            audioManager.playlevelpauseclick();
            levelstate = Levelstate.Edit;
           inputManager.isineditmode(true);
           ballmovemant.resetBallMovemant(ballstartpoint.position);
           gamestate.setplaymode();
        }
        else
        {
          if (!inputManager.ispropcollding() && !ballmovemant.getishitend())
          {
                isplay = true;
                audioManager.PlayLevelplay();
                levelstate = Levelstate.Play;
                ballmovemant.starBallMovemant();
                inputManager.isineditmode(false);
                leveltry++; 
                gamestate.setpauseicon();
          }
        }
        EventSystem.current.SetSelectedGameObject(null);
    }
    // completed state 
    void chakelevelcompaleted() 
    {
        if (ballmovemant.getishitend()) 
        {
            levelstate = Levelstate.Completed;
            levelcompaleted  = true;
            calculatethascore();
        }
    }

    public bool islevelcompaleted() { return levelcompaleted; }


    public void loadnextlevel()
    {
        int nextlevel = curentlevel + 1;
        if (nextlevel <= 10)
        {
            StartCoroutine(Loadnextlevel(nextlevel));
        }
    }

    IEnumerator Loadnextlevel(int level) 
    {
       
        gamestate.openlevelloadpanle();
    
        yield return new WaitForSeconds(0.5f);
       
            unloadlevel();
            loadlevel(level);
        
        
        yield return new WaitUntil(() => islevelload);
        gamestate.closelevelloadpanle();
    }


    // load state 

    public void loadlevel(int levelnum)
    {
        if (!islevelload && levelstate == Levelstate.Wait)
        {
            StartCoroutine(LoadLevelCoroutine(levelnum));
        }
    }

    IEnumerator LoadLevelCoroutine(int levelnum)
    {
        if (levelnum == 0)
            gamestate.opentutorealpanel(false);

        audioManager.palymenuclick();

        levelkey = $"Level{levelnum}_stars";

        if (PlayerPrefs.HasKey(levelkey))
        {
            oldstars = PlayerPrefs.GetInt(levelkey);
            newstars = 0;
        }
        else
        {
            PlayerPrefs.SetInt(levelkey, 0);
            PlayerPrefs.Save();

            oldstars = 0;
            newstars = 0;
        }

        curentlevel = levelnum;

        string path =
            Application.streamingAssetsPath +
            "/Levels/Level" +
            levelnum +
            ".json";

        UnityWebRequest request =
            UnityWebRequest.Get(path);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                "Failed to load level " +
                levelnum +
                ": " +
                request.error +
                "\nPath: " +
                path
            );

            yield break;
        }

        string leveljson =
            request.downloadHandler.text;

        Level loaddata =
            JsonUtility.FromJson<Level>(leveljson);

        maxleveltry =
            loaddata.leveltry;

        maxlevelbounce =
            loaddata.levelmaxbouncecount;

        ballstartpoint.position =
            loaddata.ballstartposition;

        ballmovemant.resetBallMovemant(
            ballstartpoint.position
        );

        levelendpont.position =
            loaddata.levelendposition;

        levelendpont.rotation =
            Quaternion.Euler(
                loaddata.levelendrotation
            );

        inputManager.Setcamerapenrange(
            loaddata.cameraxypenrange
        );

        foreach (var tempprop in loaddata.prop)
        {
            setupprop(
                tempprop.type,
                tempprop.location,
                tempprop.rotation
            );

            propdata.Add(tempprop);
        }

        foreach (
            var tempstaticobject
            in loaddata.Staticobject)
        {
            setupstaticobject(
                tempstaticobject.type,
                tempstaticobject.location,
                tempstaticobject.rotation,
                tempstaticobject.scale
            );
        }

        Debug.Log(loaddata.levelnum);

        islevelload = true;
        levelstate = Levelstate.Edit;

        inputManager.isineditmode(true);
        gamestate.setplaymode();

        leveltry = 0;
    }

    public bool getislevelload() {  return islevelload; }

    void setupprop(string proptype,Vector3 proplocation,Vector3 proprotation) 
    {
        GameObject tempprop = null;
        Debug.Log(proptype);
        switch (proptype)
        {
            case "Normal":
                tempprop = Instantiate(normal, proplocation, Quaternion.Euler(proprotation ));
                
                break;
        }
        tempprop.GetComponent<PropManager>().setstartlocationandrotation(proplocation,proprotation);
        props.Add(tempprop);
    }

    void setupstaticobject(string staticobjecttype, Vector3 staticobjectlocation, Vector3 staticobjectrotation,Vector3 staticobjectscale)
    {
        GameObject tempobj = null;
        switch (staticobjecttype)
        {
            case "Wall":
                tempobj = Instantiate(wall, staticobjectlocation, Quaternion.Euler(staticobjectrotation));
                tempobj.transform.localScale = staticobjectscale;
                break;
        }
        staticobject.Add(tempobj);
    }


    // reset state 
    public void resetlevel() 
    {
        if (levelstate == Levelstate.Completed) { return; }
        isplay = false;
        audioManager.playlevelpauseclick();
        inputManager.ResetSelection();
        inputManager.isineditmode(true);
        gamestate.setplaymode();
        ballmovemant.resetBallMovemant(ballstartpoint.position);
        foreach (var prop in props)
        {
            prop.GetComponent<PropManager>().resetpositionandrotation();
        }
        levelstate = Levelstate.Edit;
        leveltry = 0;
    }

    public void retryLevel() 
    {
        
        ballmovemant.resetBallMovemant(ballstartpoint.position);

        for (int i = 0; i < props.Count; i++) 
        {
            GameObject tempprop = props[i];
            tempprop.GetComponent<PropManager>().resetpositionandrotation();

        }

        isplay = false;
        audioManager.playlevelpauseclick();
        levelcompaleted = false;
        levelstate = Levelstate.Edit;
        inputManager.ResetSelection();
        inputManager.isineditmode(true);
        gamestate.setplaymode();
        leveltry = 0;
    }


    void calculatethascore()
    {

        int bounce = ballmovemant.getbounce();

        float tryratio = (float)leveltry / maxleveltry;
        float trypanlty = Mathf.Max(0,tryratio - 1f);
        float bounceratio= (float)bounce / maxlevelbounce;
        float bouncepanlty = Mathf.Max(0, bounceratio - 1f);
        float score = 1 + trypanlty + bouncepanlty;

        if (score <= 1) { newstars = 5; }
        else if (score <= 2) { newstars = 4; }
        else if (score <= 3) { newstars = 3; }
        else if (score <= 4) { newstars = 2; }
        else if (score <= 5) { newstars = 1; }
        else { newstars = 0; }


      
            PlayerPrefs.SetInt(levelkey, newstars);
            PlayerPrefs.Save();
        

    }

    public void levelpause() 
    {
            if (Time.timeScale > 0 && isplay)
            {
                Time.timeScale = 0;
            }
        
    }
    public void levelresume() 
    {
        Time.timeScale = 1;
    }

    public int getlevelnum() 
    {
        return curentlevel;
    }
    public string getlevelkey() 
    {
        return levelkey;
    }

    public int getleveltry() 
    {
        return leveltry;
    }
    public int getballbounce() 
    {
        return ballmovemant.getbounce();
    }
  


}
