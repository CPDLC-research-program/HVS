using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Hdg : MonoBehaviour, Field
{
    Image compass;
    Image bug;
    Text hdgVal;
    Text targetVal;

    Global global;
    DataManager dataManager;

    public Material mat;


    //0: normal
    //1: �dition
    public int mode;

    public float currentHdg;
    public float targetHdg;
    float lastTarget;
    float oldTargetHdg;

    float nextActionTime = 0.0f;
    float period = 0.02f;
    //float changeSpeed = (float)0.01;

    //Mouvement de la boussole
    bool movementStarted = false;

    //Mode direct
    bool direct = false;

    void changeUiMode()
    {
        Image box = GameObject.Find("Canvas/Heading/Hdg Target Box").GetComponent<Image>();
        Text value = GameObject.Find("Canvas/Heading/Hdg Target Value").GetComponent<Text>();
        if (mode == 0)
        {
            box.color = Color.white;
            box.material = null;
            value.color = new Color(63.0f / 255.0f, 220.0f / 255.0f, 241.0f / 255.0f);
        }
        else
        {
            box.color = new Color(63.0f / 255.0f, 220.0f / 255.0f, 241.0f / 255.0f);
            box.material = mat;
            value.color = new Color(61.0f / 255.0f, 61.0f / 255.0f, 61.0f / 255.0f); ;
        }
    }

    public int toggleMode()
    {
        if (mode == 0)
        {
            if (global.target != 4)
            {
                ++dataManager.wrongFieldSelected;
                global.playSound(1);
                StartCoroutine(global.failure(4));
                return -1;
            }

            dataManager.enterPhase0Data();

            mode = 1;

            //Changer le mode visuel
            changeUiMode();

            oldTargetHdg = targetHdg;
        }
        else
        {
            mode = 0;
            changeUiMode();
        }

        return 0;
    }

    public void editTarget(float value, bool overshoot = true)
    {
        //Premier frame en mode �dition
        if (mode == 0)
        {
            toggleMode();
        }

        //Limiter les valeurs
        //(va �tre utile pour les incr�ments)
        targetHdg = Mathf.Clamp(value, 0, 359);

        if (overshoot)
        {
            dataManager.enterOvershootValue(targetHdg);
        }
    }

    //0: �chec
    //1: succ�s
    public int confirmTarget()
    {
        if (targetHdg != global.targetHdg)
        {
            global.playSound(1);
            return 0;
        }
        else
        {
            global.targetHdgReached = true;
            global.playSound(0);
            return 1;
        }
    }

    public void restoreOldTarget()
    {
        targetHdg = oldTargetHdg;
    }

    public float getCurrentValue()
    {
        return currentHdg;
    }

    public int getMode()
    {
        return mode;
    }

    // Start is called before the first frame update
    void Start()
    {
        compass = GameObject.Find("Canvas/Heading/Moving").GetComponent<Image>();
        bug = GameObject.Find("Canvas/Heading/Moving/Hdg Bug").GetComponent<Image>();
        hdgVal = GameObject.Find("Canvas/Heading/Hdg Value").GetComponent<Text>();
        targetVal = GameObject.Find("Canvas/Heading/Hdg Target Value").GetComponent<Text>();
        global = GameObject.Find("Global").GetComponent<Global>();
        dataManager = GameObject.Find("Global").GetComponent<DataManager>();

        currentHdg = 0;
        targetHdg = 0;

        if (SceneManager.GetActiveScene().name == "TouchDirect")
        {
            direct = true;
        }
    }

    public int closestSide(float current, float target)
    {
        float clockwise;
        float anticlockwise;

        if (current == target)
        {
            return 0;
        }
        else if (current < target)
        {
            anticlockwise = target - current;
            clockwise = 360 - target + current;
        }
        else
        {
            clockwise = current - target;
            anticlockwise = 360 - current + target;
        }

        //retourner la plus petite distance 
        //1 =  horaire
        //-1 = antihoraire
        return clockwise > anticlockwise ? -1 : 1;
    }

    float getNextHdg(bool increase, float current)
    {
        if (increase)
        {
            if (current >= 359)
            {
                return 0;
            }
            else if (current > targetHdg)
            {
                return ++current;
            }
            else
            {
                return ++current > targetHdg ? targetHdg : current;
            }
            
        }
        else
        {
            if (current <= 1)
            {
                return (float)359.99;
            }
            else if (current < targetHdg)
            {
                return --current;
            }
            else
            {
                return --current < targetHdg ? targetHdg : current;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;
            currentHdg = currentHdg % 360;
            targetHdg = targetHdg % 360 ;
            hdgVal.text = Mathf.RoundToInt(currentHdg == 0 ? 360 : currentHdg).ToString();
            targetVal.text = Mathf.RoundToInt(targetHdg == 0 ? 360 : targetHdg).ToString();

            //pour r�gler le bug du bug qui ne bouge pas
            if(lastTarget != targetHdg)
            {
                if (direct)
                {
                    compass.rectTransform.eulerAngles = new Vector3(0, 0, targetHdg);
                    bug.rectTransform.eulerAngles = new Vector3(0, 0, 0);
                }
                else
                {
                    bug.rectTransform.eulerAngles = new Vector3(0, 0, -targetHdg + currentHdg);
                }
            }

            if (mode == 0)
            {
                int movement = closestSide(currentHdg, targetHdg);
                if (movement == 0)
                {
                    //Débloquer les autres actions à la fin du mouvement
                    if (movementStarted)
                    {
                        movementStarted = false;
                        global.actionInProgress = false;
                    }                    
                }
                //Sens antihoraire
                if (movement == -1)
                {
                    //Bloquer les autres actions
                    global.actionInProgress = true;

                    movementStarted = true;

                    currentHdg = getNextHdg(true, currentHdg);
                    compass.rectTransform.eulerAngles = new Vector3(0, 0, currentHdg);
                }
                //Sens horaire
                else if (movement == 1)
                {
                    //Bloquer les autres actions
                    global.actionInProgress = true;

                    movementStarted = true;

                    currentHdg = getNextHdg(false, currentHdg);
                    compass.rectTransform.eulerAngles = new Vector3(0, 0, currentHdg);
                }
            }
            
            lastTarget = targetHdg;
        }
    }
}
