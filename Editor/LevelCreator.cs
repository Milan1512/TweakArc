using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelCreator : MonoBehaviour
{

    bool islevelloaded  = false;


    public int levelnum = 0;
    public int leveltry = 0;
    public int levelmaxbouncecount = 0;
    public Vector2 cameraxypenrange;

    public Transform ballstartposition;
    public Transform levelend;



    public List<GameObject> levelprop ;
    public List<GameObject> staticobject ;

    [Header("prop tyep")]
    public GameObject normalpropPrefab = null;
    
    
    [Header("object tyep")]
    public GameObject WallobjectPrefab = null;


    public int getproplistcount() 
    {
        return levelprop.Count;
    }

    public int getStaticobjectlistcount() 
    {
        return staticobject.Count;
    }

    public void clearlevel()
    {
        if (levelprop.Count > 0)
        {
            foreach (var item in levelprop)
            {
                DestroyImmediate(item);
            }
        }
        if(staticobject.Count > 0) 
        {
            foreach (var item in staticobject)
            {
                DestroyImmediate(item);
            }
        }
        clearproplist();
        clearstaticObjectlist();
    }






    // ballstart and levelend

    public void setballstartloaction(Vector3 newballloaction) 
    {
        ballstartposition.position = newballloaction;
    }

    public void setlevelend(Vector3 postion , Vector3 rotation) 
    {
        levelend.position = postion;
        levelend.rotation = Quaternion.Euler(rotation);
    }


    public Vector3 getballstartlocation() 
    {
        return ballstartposition.position;
    }

    public Vector3 getlevelendlocaiton() 
    {
        return levelend.position;
    }

    public Vector3 getklevelendrotation() 
    {
        return levelend.eulerAngles;
    }



    // staticobject

    public void setstaticobjectlist(string tyep, Vector3 objectlocation, Vector3 objectrotation , Vector3 objectscale)
    {
        GameObject newstaticobject = null;
        switch (tyep)
        {
            case "Wall":

                newstaticobject = (GameObject)PrefabUtility.InstantiatePrefab(WallobjectPrefab);
                newstaticobject.transform.position = objectlocation;
                newstaticobject.transform.rotation = Quaternion.Euler(objectrotation);
                newstaticobject.transform.localScale = objectscale; 
                staticobject.Add(newstaticobject);
                break;
            default:
                Debug.LogWarning("unknow object tyep");
                break;
        }

    }

    public void clearstaticObjectlist() 
    {
        staticobject.Clear();
    }


    public void getallstaticobjectfromthascane()
    {
        StaticObjectManager[] StaticObjects = FindObjectsOfType<StaticObjectManager>();

        foreach (var item in StaticObjects)
        {
            staticobject.Add(item.GameObject());
        }


    }

    public string getStacicobjecttype(int num)
    {
        return staticobject[num].GetComponent<StaticObjectManager>().getobjecttyep();
    }

    public Vector3 getStaticobjectlocation(int num) 
    {
        Vector3 location = staticobject[num].transform.position;
        return location;
    }

    public Vector3 getStaticobjectrotation(int num)
    {
        Vector3 rotation = staticobject[num].transform.eulerAngles;
        return rotation;
    }
    public Vector3 getStaticobjectscale(int num)
    {
        Vector3 scale = staticobject[num].transform.localScale;
    
        return scale;
    }

    //prop function
    public void setproplist(string tyep,Vector3 proplocation,Vector3 proprotation)  
    {
        GameObject newprop = null;
        switch (tyep)
        {
            case "Normal":
                 
                newprop =  (GameObject)PrefabUtility.InstantiatePrefab(normalpropPrefab);
                newprop.transform.position = proplocation;
                newprop.transform.rotation = Quaternion.Euler(proprotation);
                levelprop.Add(newprop);
                break;
            default:
                Debug.LogWarning("unknow prop tyep");
                break;
        }
        
    } 
    public void clearproplist() 
    {
        levelprop.Clear();
    }

    public void getallpropfromthascane()
    {
        PropManager[] levelprops = FindObjectsOfType<PropManager>();

        foreach (var item in levelprops)
        {
            levelprop.Add(item.GameObject());
        }


    }


    public string getProptype(int num) 
    {
        return levelprop[num].GetComponent<PropManager>().getproptyep();
    }

    public Vector3 getProplocation(int num) 
    {
        Vector3 location = levelprop[num].transform.position;
        return location;
    }

    public Vector3 getProprotation(int num)
    {
        Vector3 rotation = levelprop[num].transform.eulerAngles;
        return rotation;
    }



}
