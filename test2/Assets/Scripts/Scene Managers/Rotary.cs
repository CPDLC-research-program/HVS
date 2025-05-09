using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rotary : MonoBehaviour
{
    VsTape vs;
    SpeedTape speed;
    AltTape alt;
    Hdg hdg;
    BaroBox baro;

    float lastRightTurn = 0.0f;
    float lastLeftTurn = 0.0f;

    bool buttonHeld = false;
    float buttonTimer;

    Global global;
    DataManager dataManager;

    void manageSpaceBar()
    {
        if (global.actionInProgress)
        {
            return;
        }

        int mode;
        Field f;

        switch (global.highlightedField)
        {
            //IAS
            case 0:
                mode = speed.mode;
                f = speed;
                break;
            //ALT
            case 1:
                mode = alt.mode;
                f = alt;
                break;
            //VS
            case 2:
                mode = vs.mode;
                f = vs;
                break;
            //BARO
            case 3:
                mode = baro.mode;
                f = baro;
                break;
            //HDG
            default:
                mode = hdg.mode;
                f = hdg;
                break;
        }

        if (mode == 0)
        {
            if (!buttonHeld)
            {
                f.toggleMode();
            }
        }
        else
        {
            //Si c'est un clic
            if (!buttonHeld)
            {
                int result = f.confirmTarget();

                //�chec
                if (result == 0)
                {
                    ++dataManager.nbValError;

                    //Remettre le pfd en mode navigation
                    //f.toggleMode();

                    //Remettre l'ancienne cible
                    //f.restoreOldTarget();

                    //Feedback d'�chec pfd
                    StartCoroutine(global.failure(global.highlightedField));
                }
                //succ�s
                else
                {
                    //Remettre le pfd en mode navigation
                    f.toggleMode();

                    //Feedback de succ�s pfd
                    StartCoroutine(global.success(global.highlightedField));
                }
            }
            //Si c'est un long press
            else
            {
                //Remettre le pfd en mode navigation
                f.toggleMode();

                //Remettre l'ancienne cible
                f.restoreOldTarget();
            }
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        vs = GameObject.Find("Canvas/Vs Tape").GetComponent<VsTape>();
        speed = GameObject.Find("Canvas/Speed Tape").GetComponent<SpeedTape>();
        alt = GameObject.Find("Canvas/Alt Tape").GetComponent<AltTape>();
        hdg = GameObject.Find("Canvas/Heading").GetComponent<Hdg>();
        baro = GameObject.Find("Canvas/Baro").GetComponent<BaroBox>();

        global = GameObject.Find("Global").GetComponent<Global>();
        dataManager = GameObject.Find("Global").GetComponent<DataManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space") || Input.GetKeyDown("joystick 2 button 2"))
        {
            buttonTimer = Time.time;
        }

        if (Input.GetKey("space") || Input.GetKey("joystick 2 button 2"))
        {
            if (Time.time - buttonTimer > 1.0f)
            {
                buttonHeld = true;
                manageSpaceBar();
            }
        }

        if (Input.GetKeyUp("space") || Input.GetKeyUp("joystick 2 button 2"))
        {
            manageSpaceBar();
            buttonHeld = false;
        }

        if (Input.GetKeyDown("right") || Input.GetKeyDown("joystick 2 button 1"))
        {
            //Rate-aiding (TODO: � ajuster avec l'encoder)
            if (Time.time - lastRightTurn <= 0.01f)
            {
                global.getNextMovement(1, 1, true);
            }
            else
            {
                global.getNextMovement(1, 0, true);
            }
            lastRightTurn = Time.time;
            lastLeftTurn = 0.0f;
        }
        if (Input.GetKeyDown("left") || Input.GetKeyDown("joystick 2 button 0"))
        {
            if (Time.time - lastLeftTurn <= 0.01f)
            {
                global.getNextMovement(-1, 1, true);
            }
            else
            {
                global.getNextMovement(-1, 0, true);
            }
            lastLeftTurn = Time.time;
            lastRightTurn = 0.0f;
        }
    }
}
