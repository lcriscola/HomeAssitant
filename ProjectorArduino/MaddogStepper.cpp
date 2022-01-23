// 
// 
// 

#include "Stepper.h"
#include "MaddogStepper.h"

MaddogStepper::MaddogStepper(int directionPin, int stepPin, int speed = 500, int steps = 200)
{
	pinMode(5, OUTPUT);
	digitalWrite(5, LOW);

	_stepper = new Stepper(steps, directionPin, stepPin);
	_stepper->setSpeed(speed);

}

void MaddogStepper::MoveClockwise(int steps = 0)
{
	_stepstoRun = steps;
	_stepsCounter = 0;
	_clockWise = true;
}

void MaddogStepper::MoveCounterClockWise(int steps = 0)
{
	_stepstoRun = steps;
	_stepsCounter = 0;
	_clockWise = false;
}

void MaddogStepper::Stop() {
	_stepstoRun = -1;
	_stepsCounter = 0;
}
void MaddogStepper::Run()
{
	if (_stepstoRun == -1)
		return;

	int steps = 0;
	if (_clockWise > 0)
	{
		steps = -1;
	}
	else {
		steps = 1;
	}
	_stepsCounter += abs(steps);
	if (_stepstoRun != 0) {
		if (_stepsCounter > _stepstoRun)
		{
			this->Stop();
			return;
		}
	}
	_stepper->step(steps);

}
