using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class DataManagerBorg : MonoBehaviour
{
    public StreamWriter writer;

    //Start
    public int participant;
    public int device;

    public float shoulder;
    public float neck;
    public float upperBack;
    public float lowerBack;
    public float arm;
    public float hand;

    void openWriter()
    {
        string filePath = Application.dataPath + "/Data/participant" + participant + "_borg.csv";

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
            "shoulder,neck,upperBack,lowerBack,arm,hand");
    }

    public void closeWriter()
    {
        writer.Close();
    }

    public void writeAnswers()
    {
        writer.WriteLine(participant.ToString() +
            "," + device.ToString().Replace(',', '.') +
            "," + shoulder.ToString().Replace(',', '.') +
            "," + neck.ToString().Replace(',', '.') +
            "," + upperBack.ToString().Replace(',', '.') +
            "," + lowerBack.ToString().Replace(',', '.') +
            "," + arm.ToString().Replace(',', '.') +
            "," + hand.ToString().Replace(',', '.'));

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
