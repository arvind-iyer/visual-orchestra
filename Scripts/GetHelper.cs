using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
[Serializable]
public class M2XSchema : MonoBehaviour
{
    public float Value { get; set; }

    public static M2XSchema CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<M2XSchema>(jsonString);
    }
}

class GetHelper : MonoBehaviour
{
    class ABC
    {
        public string Name;

    }
    void Start() { }
    
    public WWW GET(string url)
    {
        
        WWW www = new WWW(url);
        StartCoroutine(WaitForRequest(www));
        return www;
    }
    public M2XSchema m2x()
    {
        string url = "http://api-m2x.att.com/v2/devices/a24ae3830e6418252dba44f657265be8/streams/outfingers";
        WWW www = new WWW(url);
        
        StartCoroutine(WaitForRequest(www));
        while (!www.isDone) ;
        Debug.Log(www.text);
        Debug.Log("WWWTEXT :" + www.text);

        float value = float.Parse(new JSONObject(www.text)["value"].ToString());
         
        return new M2XSchema() {Value = value};
            
    }
    public WWW POST(string url, Dictionary<string, string> post)
    {
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(url, form);
        StartCoroutine(WaitForRequest(www));
        return www;
    }
    private IEnumerator WaitForRequest(WWW www)
    {
        yield return www;
        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
        Debug.Log("End req");
    }
}                                                  