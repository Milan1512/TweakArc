using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using UnityEngine.UI;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorEditor:Editor
{
 
   
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelCreator level = (LevelCreator)target;
       
        if (GUILayout.Button("loadscan"))
        {
            if (level.getproplistcount() == 0 && level.getStaticobjectlistcount() == 0)
            {
                level.getallpropfromthascane();
                level.getallstaticobjectfromthascane();
            }
            else
            {
                Debug.LogWarning("Clear the inspector first.");
            }
        }
        
        



        if (GUILayout.Button("Create level"))
        {
            Level newLevel = new Level();

            newLevel.levelnum = level.levelnum;
            newLevel.leveltry = level.leveltry;
            newLevel.levelmaxbouncecount = level.levelmaxbouncecount;
            newLevel.cameraxypenrange = level.cameraxypenrange;
            newLevel.ballstartposition = level.getballstartlocation();
            newLevel.levelendposition = level.getlevelendlocaiton();
            newLevel.levelendrotation = level.getklevelendrotation();
    


            for (int  i = 0; i < level.getproplistcount(); i++)
            {
               
                Propdata tempprop = new Propdata();
                tempprop.type = level.getProptype(i);
                tempprop.location = level.getProplocation(i);
                tempprop.rotation = level.getProprotation(i);
                newLevel.prop.Add(tempprop);
            }

            for (int i = 0; i < level.getStaticobjectlistcount(); i++)
            {
                Staticobjectdata tempobj = new Staticobjectdata();
                tempobj.type = level.getStacicobjecttype(i);
                tempobj.location=level.getStaticobjectlocation(i);
                tempobj.rotation = level.getStaticobjectrotation(i);
                tempobj.scale = level.getStaticobjectscale(i);
                newLevel.Staticobject.Add(tempobj);
                
            }

            Debug.Log(newLevel.prop.Count);
            string json = JsonUtility.ToJson(newLevel, true);

            string path = Path.Combine(Application.streamingAssetsPath, "Levels ", "Level" + newLevel.levelnum + ".json");

            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
            Debug.Log("Saved: " + path);


        }


        
        if (GUILayout.Button("load level")) 
        {

            if (level.getproplistcount() == 0 && level.getStaticobjectlistcount() == 0)
            {
                Level newLevel = new Level();


                string path = Path.Combine(Application.streamingAssetsPath, "Levels ", "Level" + level.levelnum + ".json");
                if (File.Exists(path))
                {

                    string leveljson = File.ReadAllText(path);

                    newLevel = JsonUtility.FromJson<Level>(leveljson);
                    level.leveltry = newLevel.leveltry;
                    level.levelmaxbouncecount = newLevel.levelmaxbouncecount;
                    level.setballstartloaction(newLevel.ballstartposition);
                    level.setlevelend(newLevel.levelendposition, newLevel.levelendrotation);
                    level.cameraxypenrange = newLevel.cameraxypenrange;

                    foreach (Propdata prop in newLevel.prop)
                    {
                        level.setproplist(prop.type, prop.location, prop.rotation);
                    }

                    foreach (Staticobjectdata obj in newLevel.Staticobject)
                    {
                        level.setstaticobjectlist(obj.type, obj.location, obj.rotation, obj.scale);
                    }


                }
                else
                {
                    Debug.LogWarning("level not exists");
                }
            }
            else
            {
                Debug.LogWarning("Clear the inspector first.");
            }

        }


        if (GUILayout.Button("clear inspacter"))
        {
            level.clearproplist();
            level.clearstaticObjectlist();

        }
        if (GUILayout.Button("clear level"))
        {
            level.clearlevel();
        }


    }



}
      