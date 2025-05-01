using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleFormExport : MonoBehaviour
{
    public static GoogleFormExport Instance { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ExportEnum(string BASE_URL, byte[] rawData)
    {
        WWW www = new WWW(BASE_URL, rawData);
        Debug.Log("WWW: " + www.ToString());
        yield return www;
    }

    static public void Export(string BASE_URL, byte[] rawData)
    {
        IEnumerator temp = Instance.ExportEnum(BASE_URL, rawData);
        Instance.StartCoroutine(temp);
    }
}
