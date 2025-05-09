using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Joysticks : MonoBehaviour
{
    VsTape vs;
    SpeedTape speed;
    AltTape alt;
    Hdg hdg;
    BaroBox baro;

    Global global;
    DataManager dataManager;

    int mode;

    float lastRightTurn = 0.0f;
    float lastLeftTurn = 0.0f;
    float rateAid = 0.01f;

    void getMode()
    {
        mode = global.fields[global.highlightedField].getMode();
    }

    void manageSpaceBar()
    {
        if (global.actionInProgress)
        {
            return;
        }

        Field f = global.fields[global.highlightedField];

        if (mode == 0)
        {
            f.toggleMode();
        }
        else
        {
        
            int result = f.confirmTarget();

            //?chec
            if (result == 0)
            {
                ++dataManager.nbValError;

                //Remettre le pfd en mode navigation
                //f.toggleMode();

                //Remettre l'ancienne cible
                //f.restoreOldTarget();

                //Feedback d'?chec pfd
                StartCoroutine(global.failure(global.highlightedField));
            }
            //succ?s
            else
            {
                //Remettre le pfd en mode navigation
                f.toggleMode();

                //Feedback de succ?s pfd
                StartCoroutine(global.success(global.highlightedField));
            }
        }


    }

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
        //Debug.Log(Input.GetJoystickNames()[1]);

        getMode();

        if (SceneManager.GetActiveScene().name == "RotaryJoystick")
        {
            if (Input.GetKeyDown("space") || Input.GetKeyDown("joystick 2 button 2"))
            {
                manageSpaceBar();
            }

            if (mode == 0)
            {
                //Up
                if (Input.GetKeyDown("w") || Input.GetKeyDown("joystick 2 button 3"))
                {
                    switch (global.highlightedField)
                    {
                        //IAS
                        case 0: 
                            break;
                        //ALT
                        case 1:
                            break;
                        //VS
                        case 2:
                            break;
                        //Baro
                        case 3:
                            global.highlightedField = 1; //vers Alt
                            break;
                        //HDG
                        case 4:
                            global.highlightedField = 0; //vers Ias
                            break;
                    }
                }

                //Down
                else if (Input.GetKeyDown("s") || Input.GetKeyDown("joystick 2 button 4"))
                {
                    switch (global.highlightedField)
                    {
                        //IAS
                        case 0: 
                            global.highlightedField = 4; //vers Hdg
                            break;
                        //ALT
                        case 1:
                            global.highlightedField = 3; //vers Baro
                            break;
                        //VS
                        case 2:
                            global.highlightedField = 3; //vers Baro
                            break;
                        //Baro
                        case 3:
                            break;
                        //HDG
                        case 4:
                            break;
                    }
                }

                //Left
                else if (Input.GetKeyDown("a") || Input.GetKeyDown("joystick 2 button 5"))
                {
                    switch (global.highlightedField)
                    {
                        //IAS
                        case 0: 
                            break;
                        //ALT
                        case 1:
                            global.highlightedField = 0; //vers Ias
                            break;
                        //VS
                        case 2:
                            global.highlightedField = 1; //vers Alt
                            break;
                        //Baro
                        case 3:
                            global.highlightedField = 4; //vers Hdg
                            break;
                        //HDG
                        case 4:
                            break;
                    }
                }

                //Right
                else if (Input.GetKeyDown("d") || Input.GetKeyDown("joystick 2 button 6"))
                {
                    switch (global.highlightedField)
                    {
                        //IAS
                        case 0: 
                            global.highlightedField = 1; //vers Alt
                            break;
                        //ALT
                        case 1:
                            global.highlightedField = 2; //vers Vs
                            break;
                        //VS
                        case 2:
                            break;
                        //Baro
                        case 3:
                            break;
                        //HDG
                        case 4:
                            global.highlightedField = 3; //vers Baro
                            break;
                    }
                }
            }

            else if (mode == 1)
            {
                if (Input.GetKeyDown("right") || Input.GetKeyDown("joystick 2 button 1"))
                {
                    if (Time.time - lastRightTurn <= rateAid)
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
                    if (Time.time - lastLeftTurn <= rateAid)
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

        else if (SceneManager.GetActiveScene().name == "Joystick")
        {
            if (Input.GetKeyDown("space") || Input.GetKeyDown("joystick 1 button 0"))
            {
                manageSpaceBar();
            }

            /*
            if (mode == 0)
            {
                if (Input.GetKeyDown("right") || Input.GetKeyDown("joystick 1 button 4"))
                {
                    global.getNextMovement(1, 0);
                    lastRightTurn = Time.time;
                }

                if (Input.GetKeyDown("left") || Input.GetKeyDown("joystick 1 button 3"))
                {
                    global.getNextMovement(-1, 0);
                    lastLeftTurn = Time.time;
                }
            }

            else if (mode == 1)
            {
                if (Input.GetKeyDown("up") || Input.GetKeyDown("joystick 1 button 2"))
                {
                    global.getNextMovement(1, 0, true);
                    lastRightTurn = Time.time;
                }

                if (Input.GetKey("up") || Input.GetKey("joystick 1 button 2"))
                {
                    if (Time.time - lastRightTurn > 0.3f)
                    {
                        global.getNextMovement(1, 1, true);
                        lastRightTurn = Time.time;
                    }
                }

                if (Input.GetKeyDown("down") || Input.GetKeyDown("joystick 1 button 1"))
                {
                    global.getNextMovement(-1, 0, true);
                    lastLeftTurn = Time.time;
                }

                if (Input.GetKey("down") || Input.GetKey("joystick 1 button 1"))
                {
                    if (Time.time - lastLeftTurn > 0.3f)
                    {
                        global.getNextMovement(-1, 1, true);
                        lastLeftTurn = Time.time;
                    }
                }
            }
            */

            if (mode == 0)
            {
                //Up
                if (Input.GetKeyDown("w") || Input.GetKeyDown("joystick 1 button 2"))
                {
                    switch (global.highlightedField)
                    {
                        //IAS
                        case 0: 
                            break;
                        //ALT
                        case 1:
                            break;
                        //VS
                        case 2:
                            break;
                        //Baro
                        case 3:
                            global.highlightedField = 1; //vers Alt
                            break;
                        //HDG
                        case 4:
                            global.highlightedField = 0; //vers Ias
                            break;
                    }
                }

                //Down
                else if (Input.GetKeyDown("s") || Input.GetKeyDown("joystick 1 button 1"))
                {
                    switch (global.highlightedField)
                    {
                        //IAS
                        case 0: 
                            global.highlightedField = 4; //vers Hdg
                            break;
                        //ALT
                        case 1:
                            global.highlightedField = 3; //vers Baro
                            break;
                        //VS
                        case 2:
                            global.highlightedField = 3; //vers Baro
                            break;
                        //Baro
                        case 3:
                            break;
                        //HDG
                        case 4:
                            break;
                    }
                }

                //Left
                else if (Input.GetKeyDown("a") || Input.GetKeyDown("joystick 1 button 3"))
                {
                    switch (global.highlightedField)
                    {
                        //IAS
                        case 0: 
                            break;
                        //ALT
                        case 1:
                            global.highlightedField = 0; //vers Ias
                            break;
                        //VS
                        case 2:
                            global.highlightedField = 1; //vers Alt
                            break;
                        //Baro
                        case 3:
                            global.highlightedField = 4; //vers Hdg
                            break;
                        //HDG
                        case 4:
                            break;
                    }
                }

                //Right
                else if (Input.GetKeyDown("d") || Input.GetKeyDown("joystick 1 button 4"))
                {
                    switch (global.highlightedField)
                    {
                        //IAS
                        case 0: 
                            global.highlightedField = 1; //vers Alt
                            break;
                        //ALT
                        case 1:
                            global.highlightedField = 2; //vers Vs
                            break;
                        //VS
                        case 2:
                            break;
                        //Baro
                        case 3:
                            break;
                        //HDG
                        case 4:
                            global.highlightedField = 3; //vers Baro
                            break;
                    }
                }
            }

            else if (mode == 1)
            {
                if (Input.GetKeyDown("up") || Input.GetKeyDown("joystick 1 button 2") || Input.GetKeyDown("joystick 1 button 4"))
                {
                    global.getNextMovement(1, 0, true);
                    lastRightTurn = Time.time;
                }

                if (Input.GetKey("up") || Input.GetKey("joystick 1 button 2") || Input.GetKey("joystick 1 button 4"))
                {
                    if (Time.time - lastRightTurn > 0.3f)
                    {
                        global.getNextMovement(1, 1, true);
                        lastRightTurn = Time.time;
                    }
                }

                if (Input.GetKeyDown("down") || Input.GetKeyDown("joystick 1 button 1") || Input.GetKeyDown("joystick 1 button 3"))
                {
                    global.getNextMovement(-1, 0, true);
                    lastLeftTurn = Time.time;
                }

                if (Input.GetKey("down") || Input.GetKey("joystick 1 button 1") || Input.GetKey("joystick 1 button 3"))
                {
                    if (Time.time - lastLeftTurn > 0.3f)
                    {
                        global.getNextMovement(-1, 1, true);
                        lastLeftTurn = Time.time;
                    }
                }
            }
        }
    }
}
