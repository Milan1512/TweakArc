using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{

    bool islevelselacted = false;

    [SerializeField] GameObject levelcanvas;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] GameObject menucanvas;
    [SerializeField] LevelManager levelmanager;
    [SerializeField] InputManager inputmanager;
    [SerializeField] GameObject levelcompaletedpanel;
    [SerializeField] GameObject levelmenupanel;
    [SerializeField] GameObject levelloadpanel;
    [SerializeField] AudioManager audiomanager = null;

    [Header("playbuttomicon")]
    [SerializeField] RawImage iconimage = null;
    [SerializeField] Texture2D playicon = null;
    [SerializeField] Texture2D pauseicon = null;
 
    [Header("levelcompaletedpanel")]
    [SerializeField] float waittime = 0.5f;
    [SerializeField] float curenttime = 0f;
    [SerializeField] TextMeshProUGUI leveltitle = null;
    [SerializeField] TextMeshProUGUI levelcompaletedtrycount = null;
    [SerializeField] TextMeshProUGUI levelcompaletedbouncecount = null;
    [SerializeField] Image[] stars = new Image[5];


    [Header("levelcompaleted canvas")]
    [SerializeField] TextMeshProUGUI trycount = null;
    [SerializeField] TextMeshProUGUI bounce = null;
  
    
    [Header("levelmenu panel ")]
    [SerializeField] TextMeshProUGUI menulevelnum = null;
    [SerializeField] TextMeshProUGUI menutrycount = null;
    [SerializeField] TextMeshProUGUI menubouncecount  = null;

   

    



    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }
    void Start()
    {
        menucanvas.SetActive(true);
       levelcanvas.SetActive(false);
       levelcompaletedpanel.SetActive(false);
        levelmenupanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    void Update()
    {
        setmenustate();
        setlevelcanvasinfo();
        setislevelcompaleted();
        
    }

    void setmenustate() 
    {
        if (levelmanager.getislevelload() && menucanvas.activeSelf)
        {
            menucanvas.SetActive(false);
            levelcanvas.SetActive(true);
            islevelselacted = true;
        }
         
        
    }
    
    void setislevelcompaleted() 
    {
        if (levelmanager.islevelcompaleted())
        {
            if(curenttime < waittime) 
            {
                curenttime += Time.unscaledDeltaTime;
                return;
            }
            levelcompaletedpanel.SetActive(true);

            string levelkey = levelmanager.getlevelkey();
            int levelnum = levelmanager.getlevelnum();

            int starscount = PlayerPrefs.GetInt(levelkey);
            leveltitle.text = ("LEVEL " + levelnum);
            for (int i = 0; i < stars.Length; i++)
            {
                
                Color starcolor = stars[i].color;
                if (i < starscount)
                {

                    starcolor.a = 1f;
                }
                else
                {
                    starcolor.a = 0.2f;

                }
                stars[i].color = starcolor;
            }
          
            levelcompaletedtrycount.text = "" + levelmanager.getleveltry();
            levelcompaletedbouncecount.text = "" + levelmanager.getballbounce();

        }
        else if (!levelmanager.islevelcompaleted() && levelcompaletedpanel.activeSelf)
        {
            levelcompaletedpanel.SetActive(false);
            curenttime = 0;
        } 
        
    }

    void setlevelcanvasinfo() 
    {
        trycount.text = ""+levelmanager.getleveltry();
        bounce.text = "" +levelmanager.getballbounce();
    }

    public void setplaymode() 
    {

            iconimage.texture = playicon;
    }

    public void setpauseicon() 
    {
        iconimage.texture = pauseicon;
    }


    public void returntomenu() 
    {
        levelmanager.unloadlevel();
        menucanvas.SetActive(true);
        levelcanvas.SetActive(false);
        levelmenupanel.SetActive(false);
        levelmanager.levelresume();
        audiomanager.palymenuclick();
    }


    public void levelmenuopenandclose() 
    {
        if (levelmenupanel.activeSelf) 
        {
            levelmenupanel.SetActive(false);
            levelmanager.levelresume();
        }
        else 
        {
            levelmenupanel.SetActive(true);
            levelmanager.levelpause();

            menulevelnum.text = "LEVEL " + levelmanager.getlevelnum();
            menutrycount.text = "" + levelmanager.getleveltry();
            menubouncecount.text = "" + levelmanager.getballbounce();
        }
        audiomanager.palymenuclick();
    }

    public void opentutorealpanel(bool input) 
    {
        if (input)
        {
            audiomanager.palymenuclick();
            tutorialPanel.SetActive(true);
        }
        else
        {
            tutorialPanel.SetActive(true);
        }
    }
    public void closetutorealpanel()
    {
        audiomanager.palymenuclick();
        tutorialPanel.SetActive(false);
    }

    public void openlevelloadpanle()
    {
        levelloadpanel.SetActive(true); 
    }
    public void closelevelloadpanle()
    {
        levelloadpanel.SetActive(false);
    }

}

