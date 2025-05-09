using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class DataManager : MonoBehaviour
{
    public StreamWriter writer;
    Global global;

    //Start
    public int participant;
    public int device;

    //0: small hdg
    //1: med hdg
    //2: large hdg
    //3: small baro
    //4: med baro
    //5: large baro
    //6: small alt
    //7: med alt
    //8: large alt
    //9: small vs
    //10: med vs
    //11: large vs
    //12: small ias
    //13: med ias
    //14: large ias
    public int val;

    //Phase 0 (lecture + navigation)
    public int wrongFieldSelected;
    public float timeNavigating;
    public float doubleTaskNavigating;

    //Phase 1 (entre le bon mode et la valeur)
    public int nbValError;
    public int nbInteractionVal;
    public int nbOvershootVal;
    public float avgOvershootAmplitudeVal;
    public float timeToVal;
    public float doubleTaskAvgToVal;

    //Le nombre de donn�es de double t�che � agr�ger et la somme de toutes les donn�es
    int doubleTaskNb;
    float doubleTaskDist;

    //Timers
    float timerPhase0;
    float timerPhase1;

    //Pour la gestion des overshoots
    float lastValue;
    float positionToTarget; //-1, 0, 1
    float lastPositionToTarget; //-1, 0, 1
    public float overshootInitialPosition; //-1, 1
    public float target;
    float max;
    float min;
    bool initialRun = true; //Avant de passer le target une fois
    List<float> over;
    List<float> under;

    float lastZone;
    float lastLastZone;

    public void enterOvershootValue(float value)
    {
        //Si c'est une cible Hdg
        if (global.target == 4)
        {
            if (value == target)
            {
                positionToTarget = 0;
            }
            else if (over.Contains(value))
            {
                positionToTarget = 1;
            }
            else if (under.Contains(value))
            {
                positionToTarget = -1;
            }
            else
            {
                positionToTarget = -2;
            }

            if (positionToTarget != lastPositionToTarget)
            {
                lastZone = lastPositionToTarget;
            }

            if (positionToTarget != 0)
            {
                if (lastZone != -2)
                {
                    if (positionToTarget == 1)
                    {
                        if (lastPositionToTarget == -1 && min != 0)
                        {
                            ++nbOvershootVal;
                            avgOvershootAmplitudeVal = min;
                            //Debug.Log("1, min: " + min);
                            min = 0;
                        }

                        if (over.IndexOf(value) + 1 > max)
                        {
                            max = over.IndexOf(value) + 1; //car index commence à 0, ce qui représente un overshoot de 1
                            //Debug.Log("Max: " + max);
                        }
                    }

                    if (positionToTarget == -1)
                    {
                        if (lastPositionToTarget == 1 && max != 0)
                        {
                            ++nbOvershootVal;
                            avgOvershootAmplitudeVal = max;
                            //Debug.Log("2, max: " + max);
                            max = 0;
                        }

                        if (under.IndexOf(value) + 1 > min)
                        {
                            min = under.IndexOf(value) + 1;
                            //Debug.Log("Min: " + min);
                        }
                    }
                }
            }
            else
            {
                if (lastPositionToTarget == -1 && min != 0) //si on a dépassé par le bas et qu'on est pas dans le premier passage (min != 0)
                {
                    ++nbOvershootVal;
                    avgOvershootAmplitudeVal += min;
                    //Debug.Log("3, min: " + min);
                    min = 0; //resetter le min
                }

                else if (lastPositionToTarget == 1 && max != 0) //si on a dépassé par le haut et qu'on est pas dans le premier passage (max != 0)
                {
                    ++nbOvershootVal;
                    avgOvershootAmplitudeVal += max;
                    //Debug.Log("4, max: " + max);
                    max = 0; //resetter le max
                }
            }
        }
        else //si pas Hdg
        {
            positionToTarget = value < target ? -1 : (value > target ? 1 : 0);

            if (positionToTarget != 0) //est un overshoot
            {
                if (!(initialRun && (positionToTarget == overshootInitialPosition))) //si on a passé le target
                {
                    if (positionToTarget == 1) //on a dépassé le target par le haut
                    {
                        if (lastPositionToTarget == -1 && !initialRun) //passé directement de -1 à 1
                        {
                            ++nbOvershootVal;
                            avgOvershootAmplitudeVal += target - min;
                            min = target; //resetter le min
                        }

                        if (value > max)
                        {
                            max = value;
                        }
                    }
                    else if (positionToTarget == -1) //on a dépassé le target par le bas
                    {
                        if (lastPositionToTarget == 1 && !initialRun) //passé directement de 1 à -1
                        {
                            ++nbOvershootVal;
                            avgOvershootAmplitudeVal += max - target;
                            max = target; //resetter le max
                        }

                        if (value < min)
                        {
                            min = value;
                        }
                    }

                    initialRun = false; //on a passé le target une fois
                }
            }
            else //position == 0
            {
                if (lastPositionToTarget == -1 && !initialRun)
                {
                    ++nbOvershootVal;
                    avgOvershootAmplitudeVal += target - min;
                    min = target; //resetter le min
                }

                else if (lastPositionToTarget == 1 && !initialRun)
                {
                    ++nbOvershootVal;
                    avgOvershootAmplitudeVal += max - target;
                    max = target; //resetter le max
                }
            }
        }

        

        lastPositionToTarget = positionToTarget;
        lastValue = value;
    }

    public void setOvershootArrays()
    {
        //Si c'est une cible Hdg
        if (global.target == 4)
        {
            over = new List<float>();
            for (int i = 1; i < 45; i++)
            {
                float newVal = (target + i) % 360;

                if (newVal == 0)
                {
                    newVal = 360;
                }

                over.Add(newVal);
            }

            under = new List<float>();
            for (int i = 1; i < 45; i++)
            {
                float newVal = target - i;

                if (newVal == 0)
                {
                    newVal = 360;
                }
                else if (newVal < 0)
                {
                    newVal = 360 + newVal;
                }

                under.Add(newVal);
            }
        }
    }

    public void enterDoubleTaskDistance(float dist)
    {
        ++doubleTaskNb;
        doubleTaskDist += dist;
    }

    public void startTimerPhase0()
    {
        timerPhase0 = Time.time;
    }

    public void enterPhase0Data()
    {
        //Inscrire puis resetter les donn�es de double t�che
        doubleTaskNavigating = doubleTaskDist / doubleTaskNb;
        doubleTaskDist = 0;
        doubleTaskNb = 0;

        timeNavigating = Time.time - timerPhase0;

        timerPhase1 = Time.time;

        //Resetter les extrêmes pour overshoot (ici pour s'assurer qu'ils sont bien définis)
        if (global.target == 4)
        {
            min = 0;
            max = 0;
        }
        else
        {
            min = target;
            max = target;
        }

        setOvershootArrays();
    }

    public void enterPhase1Data()
    {
        doubleTaskAvgToVal = doubleTaskDist / doubleTaskNb;

        if (nbOvershootVal != 0) //pour évter division par 0
        {
            avgOvershootAmplitudeVal = avgOvershootAmplitudeVal / nbOvershootVal;
        }

        timeToVal = Time.time - timerPhase1;
    }

    void getDevice()
    {
        string deviceName = SceneManager.GetActiveScene().name;

        switch (deviceName)
        {
            case "Joystick":
                device = 0;
                break;
            case "RotaryEncoder":
                device = 1;
                break;
            case "RotaryJoystick":
                device = 2;
                break;
            case "TouchColocKeyboard":
                device = 3;
                break;
            case "TouchDirect":
                device = 4;
                break;
            case "TouchGroupedKeyboard":
                device = 5;
                break;
            case "TouchGroupedSlider":
                device = 6;
                break;
        }
    }

    void openWriter()
    {
        string filePath = Application.dataPath + "/Data/participant" + participant + ".csv";

        if (File.Exists(filePath))
        {
            writer = new StreamWriter(filePath, append: true);
            return;
        }

        File.Create(filePath).Close();


        //Debug.Log(filePath);

        //This is the writer, it writes to the filepath
        writer = new StreamWriter(filePath);
        writer.WriteLine("participant,device,val," +
            "wrongFieldSelected,timeNavigating,doubleTaskNavigating," +
            "nbValError,nbInteractionVal,nbOvershootVal,avgOvershootAmplitudeVal,timeToVal,doubleTaskAvgToVal");
    }

    public void closeWriter()
    {
        writer.Close();
    }

    void resetVariables()
    {
        val = 0; //pas n�cessaire

        //Phase 0 (avant le bon mode)
        wrongFieldSelected = 0;
        timeNavigating = 0;
        doubleTaskNavigating = 0;

        //Phase 1 (entre le bon mode et la 1ere valeur, ou la 2e et la 1ere valeur)
        nbValError = 0;
        nbInteractionVal = 0;
        nbOvershootVal = 0;
        avgOvershootAmplitudeVal = 0;
        timeToVal = 0;
        doubleTaskAvgToVal = 0;

        doubleTaskNb = 0;
        doubleTaskDist = 0;

        timerPhase0 = 0;
        timerPhase1 = 0;

        lastPositionToTarget = -10;
        initialRun = true;
    }

    public void writeTask()
    {
        writer.WriteLine(participant.ToString() +
            "," + device.ToString() +
            "," + val.ToString() +
            "," + wrongFieldSelected.ToString() +
            "," + timeNavigating.ToString().Replace(',', '.') +
            "," + doubleTaskNavigating.ToString().Replace(',', '.') +
            "," + nbValError.ToString() +
            "," + nbInteractionVal.ToString() +
            "," + nbOvershootVal.ToString() +
            "," + avgOvershootAmplitudeVal.ToString("n2").Replace(',', '.') +
            "," + timeToVal.ToString().Replace(',', '.') +
            "," + doubleTaskAvgToVal.ToString().Replace(',', '.'));

        resetVariables();
        writer.Flush();
    }

    // Start is called before the first frame update
    void Start()
    {
        global = GameObject.Find("Global").GetComponent<Global>();
        getDevice();
        openWriter();
        resetVariables();
        //writeTask();
        //closeWriter();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
