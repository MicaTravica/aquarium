// pin 2: motor
// pin 3: led zeleni
// pin 4: led crveni
// pin 5: plovak
// pin 6: dugme, reset

int State = 0;
int BobberValue = 0;
int ButtonValue = 0;
int LastButtonValue = 0;
int RedLEDValue = 0;
int MotorValue = 0;
uint16_t TimeNow = 0;
uint16_t LastActivatedTime = millis() - 5000;
int BobberCount = 0;

int Motor = 2;
int LEDGreen = 3;
int LEDRed = 4;
int Bobber = 5;
int Button = 6;

void setup() {
  pinMode(Motor, OUTPUT);
  pinMode(LEDGreen, OUTPUT);
  pinMode(LEDRed, OUTPUT);
  pinMode(Bobber, INPUT);
  pinMode(Button, INPUT);
}

void loop() {
  BobberValue = digitalRead(Bobber);
  ButtonValue = digitalRead(Button);
 
  setState();
  
  if(State == 0) {
    standBy();
  } 
  else if (State == 1){
    bobberActive();
  } 
  else if (State == 2) {
    alarm();
  } 
  else if (State == 3) {
    motorActive();
  } 
  else if (State == 4) {
    motorStopAfterHold();
  }
}

void setState() {
  if (BobberValue != 0  && BobberCount  < 3) {
    State = 1;
  }
  else if (ButtonValue != 0 && State == 2) {
    State = 0;
    BobberCount = 0;
    LastActivatedTime = millis() - 5000;
    digitalWrite(LEDRed, LOW); 
    delay(200);
  }
  else if((ButtonValue != 0 && LastButtonValue != 0) || (ButtonValue == 0 && LastButtonValue != 0  && State == 4)) {
    State = 4;
    LastButtonValue = ButtonValue;
  }
  else if (ButtonValue != 0 || MotorValue == 1) {
    State = 3;
    LastButtonValue = ButtonValue;
  }
  else if (BobberValue == 0 && BobberCount  < 3) {
    State = 0;
    BobberCount = 0;
  } 
  else if (BobberCount >= 3 && ButtonValue == 0) {
    State = 2;
  } 
}

void standBy() {
  TimeNow = millis();
  if ((TimeNow - LastActivatedTime) >= 5000){
    digitalWrite(LEDRed, HIGH); 
    delay(250);            
    digitalWrite(LEDRed, LOW);
    LastActivatedTime = millis();
  }       
}

void bobberActive () {
  BobberCount =  BobberCount + 1;
  motorStart();
  delay(250);
  motorStop();
  delay(5000);
}

void alarm () {
  TimeNow = millis();
  if(RedLEDValue == 0 && (TimeNow - LastActivatedTime) >= 1000){
    digitalWrite(LEDRed, HIGH); 
    LastActivatedTime = millis();
    RedLEDValue = 1;
  } else if(RedLEDValue == 1 && (TimeNow - LastActivatedTime) >= 500){
    digitalWrite(LEDRed, LOW); 
    LastActivatedTime = millis();
    RedLEDValue = 0;
  }     
}

void motorActive() {
  TimeNow = millis();
  if(MotorValue == 0){
    motorStart();
    LastActivatedTime = millis();
    MotorValue = 1;
    delay(200);
  } else if(MotorValue == 1 && ((TimeNow - LastActivatedTime) >= 8000 || ButtonValue != 0)){
    motorStop();
    MotorValue = 0;
    LastActivatedTime = millis() - 5000;;
    LastButtonValue = 0;
    delay(200);
  }     
}

void motorStopAfterHold() {
  if (LastButtonValue == 0) {
    motorStop();
    MotorValue = 0;
    LastActivatedTime = millis() - 5000;
  }
}

void motorStart() {
  digitalWrite(Motor, HIGH); 
  digitalWrite(LEDGreen, HIGH);
}

void motorStop() {
  digitalWrite(Motor, LOW); 
  digitalWrite(LEDGreen, LOW); 
}
