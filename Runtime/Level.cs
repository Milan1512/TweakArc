using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int levelnum;
    public int leveltry;
    public int levelmaxbouncecount;
    public Vector3 ballstartposition;
    public Vector3 levelendposition;
    public Vector3 levelendrotation;
    public Vector2 cameraxypenrange;
    public List<Propdata> prop = new List<Propdata>();
    public List<Staticobjectdata> Staticobject = new List<Staticobjectdata>();

}

[System.Serializable]
public class Propdata
{
    public string type;
    public Vector3 location;
    public Vector3 rotation;
}
[System.Serializable]
public class Staticobjectdata
{
    public string type;
    public Vector3 location;
    public Vector3 rotation;
    public Vector3 scale;
}