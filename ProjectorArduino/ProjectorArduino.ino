#include "MaddogStepper.h"
#define STEPS 200
#define DIRECTION_PIN 3
#define STEP_PIN 2

MaddogStepper stepper(DIRECTION_PIN, STEP_PIN,300 );


void setup() {
	// put your setup code here, to run once:
	Serial.begin(115200);

	//stepper.setMaxSpeed(50);
	//stepper.setSpeed(50);
}

void loop() {
	stepper.Run();
	// put your main code here, to run repeatedly:
	//Serial.println(millis());
	//delay(500);
	if (Serial.available())
	{
		char c = Serial.read();
		Serial.println(c);
		if (c == 'f') {
			stepper.MoveClockwise(800);
			//stepper.moveTo(500);
			Serial.println("forward");
		}
		if (c == 's') {
			stepper.Stop();
			//stepper.moveTo(500);
			Serial.println("stopped");
		}

		if (c == 'l') {
			stepper.MoveClockwise();
			//stepper.moveTo(500);
			Serial.println("Forward");
		}
		if (c == 'L') {
			stepper.MoveCounterClockWise();
			//stepper.moveTo(500);
			Serial.println("Forward");
		}
		if (c == 'b') {
			stepper.MoveCounterClockWise(800);
			//stepper.moveTo(-500);
			Serial.println("Backward");
		}
		//stepper.runSpeedToPosition();
	}
}