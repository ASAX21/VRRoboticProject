/* **********************************************
| Differential Drive
| Example for using MOTOR and QUAD functions for
| driving a vehicle with DIFFERENTIAL_DRIVE steering
|
| Thomas Braunl, 2004, 2017
| ------------------------------------------------ */ 
#include "eyebot.h"
  
void Mdrive(char* txt, int left, int right)
/* Print txt and drive motors and encoders for 1.5s */
{ LCDPrintf("%s\n", txt);
  MOTORDrive(1,left);
  MOTORDrive(2,right);
  OSWait(1500);
  LCDPrintf("Enc. %5d %5d\n", ENCODERRead(1), ENCODERRead(2));
}

 
int main ()
{ LCDPrintf("Diff.Steering\n");
	
  Mdrive("Forward",     60, 60);
  Mdrive("Backward",   -60,-60);
  Mdrive("Left Curve",  20, 60);
  Mdrive("Right Curve", 60, 20);
  Mdrive("Turn Spot L",-20, 20);
  Mdrive("Turn Spot R", 20,-20);
  Mdrive("Stop",         0,  0);

  return 0;
}

