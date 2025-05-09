using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedTape : MonoBehaviour, Field
{
    Image tape;
    public Image bug;
    Text speedVal;
    Text targetVal;

    AltTape alt;
    Global global;
    DataManager dataManager;

    public Material mat;

    public int mode;

    float maxHeight = (float)1491;
    float minBugHeight = (float)-1441.2;

    public float currentSpeed;
    public float targetSpeed;
    float oldTargetSpeed;

    int oldVMode;

    float nextActionTime = 0.0f;
    float period = 0.001f;
    //float period = 0.005f;
    float changeSpeed = (float)5;
    //float changeSpeed = (float)0.1;

    float pixelToSpeed(float pix)
    {
        return (float)((maxHeight - pix) / 7.2);
    }

    public float speedToPixel(float speed)
    {
        return (float)(maxHeight - (speed * 7.2));
    }

    public float bugPosition(float speed)
    {
        return (float)(minBugHeight + (speed * 7.2));
    }

    void hideBug()
    {
        bug.rectTransform.anchoredPosition = new Vector3(bug.rectTransform.anchoredPosition.x, -10000);
    }

    void changeUiMode()
    {
        Image box = GameObject.Find("Canvas/Speed Tape/Speed Target Box").GetComponent<Image>();
        Text value = GameObject.Find("Canvas/Speed Tape/Speed Target Value").GetComponent<Text>();
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
            if (global.target != 0)
            {
                ++dataManager.wrongFieldSelected;
                global.playSound(1);
                StartCoroutine(global.failure(0));
                return -1;
            }

            dataManager.enterPhase0Data();

            mode = 1;

            //Changer le mode visuel
            changeUiMode();

            oldTargetSpeed = targetSpeed;
        }
        else
        {
            //Faire un changement en 1.5 secondes
            changeSpeed = Mathf.Abs(targetSpeed - currentSpeed) / (1.5f / Time.deltaTime);

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
        targetSpeed = Mathf.Clamp(value, 0, 400);

        if (overshoot)
        {
            dataManager.enterOvershootValue(targetSpeed);
        }
    }

    //0: �chec
    //1: succ�s
    public int confirmTarget()
    {
        if (targetSpeed != global.targetSpeed)
        {
            global.playSound(1);
            return 0;
        }
        else
        {
            global.targetSpeedReached = true;
            global.playSound(0);
            return 1;
        }
    }

    public void restoreOldTarget()
    {
        targetSpeed = oldTargetSpeed;
    }

    public float getCurrentValue()
    {
        return currentSpeed;
    }

    public int getMode()
    {
        return mode;
    }

    // Start is called before the first frame update
    void Start()
    {
        tape = GameObject.Find("Canvas/Speed Tape/Moving").GetComponent<Image>();
        bug = GameObject.Find("Canvas/Speed Tape/Moving/Speed Bug").GetComponent<Image>();
        speedVal = GameObject.Find("Canvas/Speed Tape/Speed Pointer/Speed Value").GetComponent<Text>();
        targetVal = GameObject.Find("Canvas/Speed Tape/Speed Target Value").GetComponent<Text>();
        alt = GameObject.Find("Canvas/Alt Tape").GetComponent<AltTape>();
        global = GameObject.Find("Global").GetComponent<Global>();
        dataManager = GameObject.Find("Global").GetComponent<DataManager>();

        currentSpeed = 160;
        targetSpeed = float.MinValue;

        tape.rectTransform.anchoredPosition = new Vector3(tape.rectTransform.anchoredPosition.x, speedToPixel(currentSpeed));

        mode = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;
            speedVal.text = Mathf.RoundToInt(currentSpeed).ToString();
            if ((targetSpeed == float.MinValue) || (targetSpeed == currentSpeed && mode == 0))
            {
                targetSpeed = float.MinValue;
                targetVal.text = currentSpeed.ToString();
                hideBug();
            }
            else
            {
                targetVal.text = Mathf.RoundToInt(targetSpeed).ToString();
                bug.rectTransform.anchoredPosition = new Vector3(bug.rectTransform.anchoredPosition.x, bugPosition(targetSpeed));

                if (mode == 0)
                {
                    //Bloquer les autres actions
                    global.actionInProgress = true;

                    if (targetSpeed < currentSpeed)
                    {
                        currentSpeed = Mathf.Clamp(currentSpeed - changeSpeed, targetSpeed, float.PositiveInfinity);
                        tape.rectTransform.anchoredPosition = new Vector3(tape.rectTransform.anchoredPosition.x, speedToPixel(currentSpeed));
                    }
                    else if (targetSpeed > currentSpeed)
                    {
                        currentSpeed = Mathf.Clamp(currentSpeed + changeSpeed, float.NegativeInfinity, targetSpeed);
                        tape.rectTransform.anchoredPosition = new Vector3(tape.rectTransform.anchoredPosition.x, speedToPixel(currentSpeed));
                    }

                    if (targetSpeed == currentSpeed)
                    {
                        //D�bloquer les autres actions
                        global.actionInProgress = false;
                    }
                }
            }
        }
    }
}
