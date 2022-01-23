#include <Stepper.h>
//#include <AccelStepper.h>

#define STEPS 200
#define DIRECTION_PIN 3
#define STEP_PIN 2

Stepper stepper(STEPS, DIRECTION_PIN, STEP_PIN );
//AccelStepper stepper(STEP_PIN, DIRECTION_PIN);


void setup() {
	// put your setup code here, to run once:
	Serial.begin(115200);
	stepper.setSpeed(200);

	//stepper.setMaxSpeed(50);
	//stepper.setSpeed(50);
}

void loop() {
	//stepper.run();
	// put your main code here, to run repeatedly:
	//Serial.println(millis());
	//delay(500);
	if (Serial.available())
	{
		char c = Serial.read();
		Serial.println(c);
		if (c == 'f') {
			stepper.step(500);
			//stepper.moveTo(500);
			Serial.println("forward");
		}
		if (c == 'F') {
			for (size_t i = 0; i < 500; i++)
			{
				stepper.step(1);
			}
			//stepper.moveTo(500);
			Serial.println("Forward");
		}
		if (c == 'l') {
			while(true)
			{
				stepper.step(1);
			}
			//stepper.moveTo(500);
			Serial.println("Forward");
		}
		if (c == 'b') {
			stepper.step(-500);
			//stepper.moveTo(-500);
			Serial.println("Backward");
		}
		//stepper.runSpeedToPosition();
	}
}