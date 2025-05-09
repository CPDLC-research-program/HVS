using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class DataManagerNasaTlx : MonoBehaviour
{
    public StreamWriter writer;

    //Start
    public int participant;
    public int device;

    public float mental;
    public float physical;
    public float temporal;
    public float performance;
    public float effort;
    public float frustration;

    void openWriter()
    {
        string filePath = Application.dataPath + "/Data/participant" + participant + "_nasa-tlx.csv";

        if (File.Exists(filePath))
        {
            writer = new StreamWriter(filePath, append: true);
            return;
        }

        File.Create(filePath).Close();


        //Debug.Log(filePath);

        //This is the writer, it writes to the filepath
        writer = new StreamWriter(filePath);
        writer.WriteLine("participant,device," +
            "mental,physical,temporal,performance,effort,frustration");
    }

    public void closeWriter()
    {
        writer.Close();
    }

    public void writeAnswers()
    {
        writer.WriteLine(participant.ToString() +
            "," + device.ToString("n2").Replace(',', '.') +
            "," + mental.ToString("n2").Replace(',', '.') +
            "," + physical.ToString("n2").Replace(',', '.') +
            "," + temporal.ToString("n2").Replace(',', '.') +
            "," + performance.ToString("n2").Replace(',', '.') +
            "," + effort.ToString("n2").Replace(',', '.') +
            "," + frustration.ToString("n2").Replace(',', '.'));

        writer.Flush();
    }

    // Start is called before the first frame update
    void Start()
    {
        openWriter();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("space"))
        {
            writeAnswers();
            Debug.Log("Log successful");
        }
    }
}
