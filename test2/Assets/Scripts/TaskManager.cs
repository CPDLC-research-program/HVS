using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    //Codes habituels
    int taskType;

    
    int remSmallHdg = 1;
    int remMediumHdg = 1;
    int remLargeHdg = 1;

    int remSmallBaro = 1;
    int remMediumBaro = 1;
    int remLargeBaro = 1;

    int remSmallAlt = 1;
    int remMediumAlt = 1;
    int remLargeAlt = 1;

    int remSmallVs = 1;
    int remMediumVs = 1;
    int remLargeVs = 1;

    int remSmallIas = 1;
    int remMediumIas = 1;
    int remLargeIas = 1;
    

    /*
    int remSmallHdg = 2;
    int remMediumHdg = 2;
    int remLargeHdg = 2;
    int remSmallBaro = 2;
    int remMediumBaro = 2;
    int remLargeBaro = 2;
    int remSmallAlt = 2;
    int remMediumAlt = 2;
    int remLargeAlt = 2;
    int remSmallVs = 2;
    int remMediumVs = 2;
    int remLargeVs = 2;
    int remSmallIas = 2;
    int remMediumIas = 2;
    int remLargeIas = 2;
    */

    bool hdgRemaining = true;
    bool altRemaining = true;
    bool baroRemaining = true;
    bool vsRemaining = true;
    bool iasRemaining = true;

    BaroBox baro;
    Hdg hdg;
    SpeedTape speed;
    AltTape alt;
    VsTape vs;

    Text task;
    Global global;
    DataManager dataManager;

    float target; //la cible pour la tâche en cours

    Image taskHighlight;

    float nextTime = 0.0f;

    bool firstFrame = true;
    bool ended = false;

    bool showHdgDir = false;

    Color green = new Color(0.0f / 255.0f, 255.0f / 255.0f, 89.0f / 255.0f);

    void writeRemainingTasks()
    {
        global.hdgValue.text = (remSmallHdg.ToString() + " " + remMediumHdg.ToString() + " " + remLargeHdg.ToString() + " ").ToString();
        global.altValue.text = (remSmallAlt.ToString() + " " + remMediumAlt.ToString() + " " + remLargeAlt.ToString() + " ").ToString();
        global.iasValue.text = (remSmallIas.ToString() + " " + remMediumIas.ToString() + " " + remLargeIas).ToString();
        global.vsValue.text = (remSmallVs.ToString() + " " + remMediumVs.ToString() + " " + remLargeVs.ToString() + " ").ToString();
        global.baroValue.text = (remSmallBaro.ToString() + " " + remMediumBaro.ToString() + " " + remLargeBaro.ToString() + " ").ToString();
    }

    IEnumerator generateTaskAfterDelay(int seconds)
    {
        //Afficher le highlight vert de la tâche
        taskHighlight.color = green;

        writeRemainingTasks();
        yield return new WaitForSeconds(2);

        //réinitialiser le texte de tâche
        task.text = "";

        //Remettre le highlight de la tâche noir
        taskHighlight.color = Color.black;

        yield return new WaitForSeconds(seconds);
        generateNewTask();
    }

    private void resetModesTargets()
    {
        global.targetSpeed = int.MinValue;
        global.targetAlt = int.MinValue;
        global.targetVs = int.MinValue;
        global.targetHdg = int.MinValue;
        global.targetBaro = float.MinValue;

        global.targetSpeedReached = false;
        global.targetAltReached = false;
        global.targetVsReached = false;
        global.targetBaroReached = false;
        global.targetHdgReached = false;
    }

    public void writeTask()
    {
        string consigne = "";
        
        //Ias
        if (taskType == 0)
        {
            consigne += "IAS:\t" + global.targetSpeed + " kts";
        }
        //Alt
        if (taskType == 1)
        {
            consigne += "ALT:\t" + global.targetAlt + " ft";
        }
        //Vs
        if (taskType == 2)
        {
            consigne += "VS:\t" + global.targetVs + " ft/m";
        }
        //Baro
        if (taskType == 3)
        {
            consigne += "Baro:\t" + global.targetBaro.ToString("n2") + " inHg";
        }
        //Hdg
        if (taskType == 4)
        {
            consigne += "HDG:\t" + (global.targetHdg == 0 ? 360 : global.targetHdg) +
                "°" + (showHdgDir ? "\nDir:\t" + (hdg.closestSide(hdg.currentHdg, global.targetHdg) == 1 ? "L" : "R") : "");
        }

        task.text = consigne;
        
    }

    //type est le 0-4 habituel
    void getNextValue(int type, int direction = 0)
    {
        //si direction n'est pas d�fini
        if (direction == 0)
        {
            //0 : diminuer, 1: augmenter
            direction = Random.Range(0, 2);

            if (direction == 0)
            {
                direction = -1;
            }
        }
        

        //0: petit, 1: moyen, 2: grand
        int amplitude = Random.Range(0, 3);

        int value;

        switch (type)
        {
            //IAS
            case 0:
                if (amplitude == 0)
                {
                    if (remSmallIas == 0)
                    {
                        getNextValue(type);
                        return;
                    }

                    value = Random.Range(1, 11);
                    dataManager.val = 12;
                    --remSmallIas;
                }
                else if (amplitude == 1)
                {
                    if (remMediumIas == 0)
                    {
                        getNextValue(type);
                        return;
                    }

                    value = Random.Range(11, 76);
                    dataManager.val = 13;
                    --remMediumIas;
                }
                else
                {
                    if (remLargeIas == 0)
                    {
                        getNextValue(type);
                        return;
                    }

                    value = Random.Range(76, 101);
                    dataManager.val = 14;
                    --remLargeIas;
                }

                int currentSpeed = Mathf.RoundToInt(speed.currentSpeed);

                if ((value * direction) + currentSpeed > 400 || (value * direction) + currentSpeed < 0)
                {
                    direction *= -1;
                }

                value = value * direction + currentSpeed;

                global.targetSpeed = value;
                target = global.targetSpeed;

                break;
            //ALT
            case 1:

                int currentAlt = Mathf.RoundToInt(alt.currentAlt);

                if (amplitude == 0)
                {
                    if (remSmallAlt == 0)
                    {
                        getNextValue(type, direction);
                        return;
                    }

                    value = Random.Range(1, 11) * 100;
                    dataManager.val = 6;
                    --remSmallAlt;
                }
                else if (amplitude == 1)
                {
                    if (remMediumAlt == 0)
                    {
                        getNextValue(type, direction);
                        return;
                    }

                    value = Random.Range(11, 151) * 100;

                    //si la valeur fonctionne directement
                    if ((currentAlt + value * direction <= 20000) && (currentAlt + value * direction >= 0))
                    {
                        //ne rien faire
                    }
                    //si la valeur fonctionne avec le signe invers�, inverser le signe
                    else if (currentAlt + value * direction * -1 <= 20000 && currentAlt + value * direction * -1 >= 0)
                    {
                        direction = direction * -1;
                    }
                    //si on est bloqu�
                    else
                    {
                        //On prend un autre guess
                        getNextValue(type, direction);
                        return;
                    }

                    dataManager.val = 7;
                    --remMediumAlt;
                }
                else
                {
                    if (remLargeAlt == 0)
                    {
                        getNextValue(type, direction);
                        return;
                    }

                    value = Random.Range(151, 181) * 100;

                    //si la valeur fonctionne directement
                    if ((currentAlt + value * direction <= 20000) && (currentAlt + value * direction >= 0))
                    {
                        //ne rien faire
                    }
                    //si la valeur fonctionne avec le signe invers�, inverser le signe
                    else if (currentAlt + value * direction * -1 <= 20000 && currentAlt + value * direction * -1 >= 0)
                    {
                        direction = direction * -1;
                    }
                    //si on est bloqu�
                    else
                    {
                        //Si c'est le dernier mouvement possible
                        if (remSmallAlt == 0 && remMediumAlt == 0)
                        {
                            //On met une valeur pr�s d'un bout ([0, 2000[) sans d�cr�menter 
                            global.targetAlt = Random.Range(0, 20) * 100;
                            target = global.targetAlt;

                            dataManager.val = 7; //alt moyen

                            return;
                        }
                        else
                        {
                            //On prend un autre guess
                            getNextValue(type, direction);
                            return;
                        }
                    }

                    dataManager.val = 8;
                    --remLargeAlt;
                }

                if ((value * direction) + currentAlt > 20000 || (value * direction) + currentAlt < 0)
                {
                    direction *= -1;
                }

                value = value * direction + currentAlt;

                int rem = Mathf.RoundToInt(value) % 100;
                value = (rem >= 50 ? (Mathf.RoundToInt(value) - rem + 100) : Mathf.RoundToInt(value) - rem);

                global.targetAlt = value;
                target = global.targetAlt;

                break;
            //VS
            case 2:
                if (amplitude == 0)
                {
                    if (remSmallVs == 0)
                    {
                        getNextValue(type);
                        return;
                    }

                    value = Random.Range(1, 6) * 100;
                    dataManager.val = 9;
                    --remSmallVs;
                }
                else if (amplitude == 1)
                {
                    if (remMediumVs == 0)
                    {
                        getNextValue(type);
                        return;
                    }

                    value = Random.Range(6, 16) * 100;
                    dataManager.val = 10;
                    --remMediumVs;
                }
                else
                {
                    if (remLargeVs == 0)
                    {
                        getNextValue(type);
                        return;
                    }

                    value = Random.Range(16, 21) * 100;
                    dataManager.val = 11;
                    --remLargeVs;
                }

                value = value * direction;
                
                global.targetVs = value;
                target = global.targetVs;

                break;
            //BARO
            case 3:

                int currentBaro = Mathf.RoundToInt(baro.currentBaro * 100);
                
                if (amplitude == 0)
                {
                    if (remSmallBaro == 0)
                    {
                        getNextValue(type);
                        return;
                    }

                    value = Random.Range(1, 11);

                    //Debug.Log("Small: " + value);

                    //si l'ajout de la valeur exc�de les limites, inverser le signe de la valeur
                    if (currentBaro + value * direction > 3100 || currentBaro + value * direction < 2800)
                    {
                        direction = direction * -1;
                    }

                    dataManager.val = 3;
                    --remSmallBaro;
                }
                else if (amplitude == 1)
                {
                    if (remMediumBaro == 0)
                    {
                        getNextValue(type);
                        return;
                    }

                    value = Random.Range(11, 101);

                    //Debug.Log("med: " + value);

                    //si l'ajout de la valeur exc�de les limites, inverser le signe de la valeur
                    if (currentBaro + value * direction > 3100 || currentBaro + value * direction < 2800)
                    {
                        direction = direction * -1;
                    }

                    dataManager.val = 4;
                    --remMediumBaro;
                }
                else
                {
                    if (remLargeBaro == 0)
                    {
                        getNextValue(type);
                        return;
                    }

                    value = Random.Range(101, 181);

                    //Debug.Log("Large: " + value);

                    //si la valeur fonctionne directement
                    if (currentBaro + value * direction <= 3100 && currentBaro + value * direction >= 2800)
                    {
                        //ne rien faire
                    }
                    //si la valeur fonctionne avec le signe invers�, inverser le signe
                    else if (currentBaro + value * direction * -1 <= 3100 && currentBaro + value * direction * -1 >= 2800)
                    {
                        direction = direction * -1;
                    }
                    //si on est bloqu�
                    else
                    {
                        //Si c'est le dernier mouvement possible
                        if (remSmallBaro == 0 && remMediumBaro == 0)
                        {
                            //On met une valeur pr�s d'un bout sans d�cr�menter
                            global.targetBaro = (2800 + Random.Range(0, 20)) / 100;
                            target = global.targetBaro;

                            dataManager.val = 4; //baro moyen
                            return;
                        }
                        else
                        {
                            //On prend un autre guess
                            getNextValue(type);
                        }
                    }

                    dataManager.val = 5;
                    --remLargeBaro;
                }

                value = value * direction + currentBaro;

                global.targetBaro = (float)value / 100;
                target = global.targetBaro;

                break;
            //HDG
            default:
                if (amplitude == 0)
                {
                    if (remSmallHdg == 0)
                    {
                        getNextValue(type);
                        return;
                    }

                    value = Random.Range(1, 11);
                    dataManager.val = 0;
                    --remSmallHdg;
                }
                else if (amplitude == 1)
                {
                    if (remMediumHdg == 0)
                    {
                        getNextValue(type);
                        return;
                    }

                    value = Random.Range(11, 161);
                    dataManager.val = 1;
                    --remMediumHdg;
                }
                else
                {
                    if (remLargeHdg == 0)
                    {
                        getNextValue(type);
                        return;
                    }

                    value = Random.Range(161, 181);
                    dataManager.val = 2;
                    --remLargeHdg;
                }

                value = value * direction + Mathf.RoundToInt(hdg.currentHdg);

                if (value < 0)
                {
                    value = 360 + value;
                }

                global.targetHdg = value % 360;
                target = global.targetHdg;

                break;
        }

    }

    public void generateNewTask()
    {
        do
        {
            //taskType = Random.Range(0, 3);
            taskType = Random.Range(0, 5);

            //ne pas g�n�rer une t�che pour une valeur termin�e
            if ((taskType == 0 && !iasRemaining) || (taskType == 1 && !altRemaining) || (taskType == 2 && !vsRemaining) ||
                (taskType == 3 && !baroRemaining) || (taskType == 4 && !hdgRemaining))
            {
                taskType = -1;
            }
            
        } while (taskType == -1);

        //Type de tâche
        global.target = taskType;

        getNextValue(taskType);

        //Mettre le bord initial pour les overshoots
        float current = global.fields[taskType].getCurrentValue();
        
        //Changer 0 pour 360 si Hdg
        if (taskType == 4)
        {
            if (current == 0)
            {
                current = 360;
            }
        }

        dataManager.overshootInitialPosition = (current < target ? -1 : 1);
        dataManager.target = target;

        global.playSound(2);
        writeTask();

        //Starter le timer de la phase 0
        dataManager.startTimerPhase0();
    }

    void correctTask()
    {
        if ((taskType == 0 && global.targetSpeedReached) ||
            (taskType == 1 && global.targetAltReached) ||
            (taskType == 2 && global.targetVsReached) ||
            (taskType == 3 && global.targetBaroReached) ||
            (taskType == 4 && global.targetHdgReached))
        {
            dataManager.enterPhase1Data();
            dataManager.writeTask();

            global.target = -1;

            updateRemaining();

            bool ended = checkEnding();

            if (!ended)
            {
                resetModesTargets();

                //D�marrer une nouvelle t�che apr�s 5-10 secondes
                StartCoroutine(generateTaskAfterDelay(Random.Range(3, 7)));
                //StartCoroutine(generateTaskAfterDelay(Random.Range(5, 11)));
            }
            else
            {
                dataManager.closeWriter();
                task.text = "Terminé! / Finished!";
            }
        }
    }

    bool checkEnding()
    {
        if (remSmallHdg == 0 && remMediumHdg == 0 && remLargeHdg == 0 && remSmallBaro == 0 && remMediumBaro == 0 && remLargeBaro == 0 &&
            remSmallAlt == 0 && remMediumAlt == 0 && remLargeAlt == 0 && remSmallVs == 0 && remMediumVs == 0 && remLargeVs == 0 &&
            remSmallIas == 0 && remMediumIas == 0 && remLargeIas == 0)
        {
            //Feedback de fin
            //Revoir, faire plut�t � la correction
            ended = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    void updateRemaining()
    {
        if (remSmallHdg == 0 && remMediumHdg == 0 && remLargeHdg == 0)
        {
            hdgRemaining = false;
        }
        if (remSmallAlt == 0 && remMediumAlt == 0 && remLargeAlt == 0)
        {
            altRemaining = false;
        }
        if (remSmallBaro == 0 && remMediumBaro == 0 && remLargeBaro == 0)
        {
            baroRemaining = false;
        }
        if (remSmallVs == 0 && remMediumVs == 0 && remLargeVs == 0)
        {
            vsRemaining = false;
        }
        if (remSmallIas == 0 && remMediumIas == 0 && remLargeIas == 0)
        {
            iasRemaining = false;
        }
    }

    void addSomeMore()
    {
        List<int> chosen = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            int extra;

            do
            {
                extra = Random.Range(0, 15);
            } while (chosen.Contains(extra));

            chosen.Add(extra);

            switch (extra)
            {
                case 0:
                    ++remSmallHdg;
                    break;
                case 1:
                    ++remMediumHdg;
                    break;
                case 2:
                    ++remLargeHdg;
                    break;
                case 3:
                    ++remSmallAlt;
                    break;
                case 4:
                    ++remMediumAlt;
                    break;
                case 5:
                    ++remLargeAlt;
                    break;
                case 6:
                    ++remSmallBaro;
                    break;
                case 7:
                    ++remMediumBaro;
                    break;
                case 8:
                    ++remLargeBaro;
                    break;
                case 9:
                    ++remSmallIas;
                    break;
                case 10:
                    ++remMediumIas;
                    break;
                case 11:
                    ++remLargeIas;
                    break;
                case 12:
                    ++remSmallVs;
                    break;
                case 13:
                    ++remMediumVs;
                    break;
                case 14:
                    ++remLargeVs;
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        hdg = GameObject.Find("Canvas/Heading").GetComponent<Hdg>();
        baro = GameObject.Find("Canvas/Baro").GetComponent<BaroBox>();
        vs = GameObject.Find("Canvas/Vs Tape").GetComponent<VsTape>();
        speed = GameObject.Find("Canvas/Speed Tape").GetComponent<SpeedTape>();
        alt = GameObject.Find("Canvas/Alt Tape").GetComponent<AltTape>();

        task = GameObject.Find("Canvas/Task").GetComponent<Text>();
        global = GameObject.Find("Global").GetComponent<Global>();
        dataManager = GameObject.Find("Global").GetComponent<DataManager>();

        taskHighlight = GameObject.Find("Canvas/Task/Task Highlight").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (firstFrame)
        {
            addSomeMore();

            if (dataManager.device == 1 || dataManager.device == 2)
            {
                showHdgDir = true;
            }

            writeRemainingTasks();
            generateNewTask();

            firstFrame = false;
        }

        if ((Time.time - nextTime > 0.5f) && !ended)
        {
            correctTask();
        }
        
    }
}