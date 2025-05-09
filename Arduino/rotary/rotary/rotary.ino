#include <Joystick.h>

//encoder
#define dt_pin 3 //4 dans le spec sheet
#define clk_pin 4 //5

volatile int rotation = 0;

int previousClk;
int currentClk;
int previousDt;
int currentDt;

//bouton
int button_1 = 8; //2
int button_2 = 9; //3 dans spec sheet

Joystick_ Joystick(JOYSTICK_DEFAULT_REPORT_ID,JOYSTICK_TYPE_GAMEPAD,
  3, 0,                  // Button Count, Hat Switch Count
  false, false, false,     // X and Y, but no Z Axis
  false, false, false,   // No Rx, Ry, or Rz
  false, false,          // No rudder or throttle
  false, false, false);  // No accelerator, brake, or steering

void setup() {
    Serial.begin(115200);
    while (!Serial);

    pinMode(button_1, INPUT_PULLUP);
    pinMode(button_2, OUTPUT);

    pinMode(dt_pin, INPUT_PULLUP);
    pinMode(clk_pin, INPUT_PULLUP);
    
    Joystick.begin(false);
}

void loop() {
    //encoder
    // Read the current state of inputCLK
    currentClk = digitalRead(clk_pin);

    // If the previous and the current state of the inputCLK are different then a pulse has occured
    if (currentClk != previousClk) {

        // If the inputDT state is different than the inputCLK state then 
        // the encoder is rotating counterclockwise
        if (digitalRead(dt_pin) != currentClk) {
            rotation = -1;

        }
        else {
            // Encoder is rotating clockwise
            rotation = 1;

        }
    }
    // Update previousStateCLK with the current state
    previousClk = currentClk;

    currentDt = digitalRead(dt_pin);

    // If the previous and the current state of the inputCLK are different then a pulse has occured
    if (currentDt != previousDt) {

        // If the inputDT state is different than the inputCLK state then 
        // the encoder is rotating counterclockwise
        if (digitalRead(clk_pin) != currentDt) {
            rotation = 1;

        }
        else {
            // Encoder is rotating clockwise
            rotation = -1;

        }
    }
    // Update previousStateCLK with the current state
    previousDt = currentDt;

    if (rotation != 0) {
        if (rotation == 1) {
            Serial.println("LEFT");
            Joystick.pressButton(0);
        }
        else if (rotation == -1) {
            Serial.println("RIGHT");
            Joystick.pressButton(1);
        }
        rotation = 0;
    }

    //bouton
    digitalWrite(button_2, LOW);

    int b1 = digitalRead(button_1);

    if (b1 == 0) {
        Serial.println("Bouton pesé");
        Joystick.pressButton(2);
    }
    
    Joystick.sendState();
    delay(5); // wait for a second
    ClearButtons();
}

void ClearButtons()
{
  Joystick.releaseButton(0);
  Joystick.releaseButton(1);
  Joystick.releaseButton(2);  
}
