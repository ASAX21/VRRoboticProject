/* **********************************************
| Ackermann Drive
| Example for using MOTOR and QUAD functions for
| driving a vehicle with ACKERMANN_DRIVE steering
|
| Thomas Braunl, 2004, 2017
| -----------------------------------------------*/ 
#include "eyebot.h"

void Mdrive(char* txt, int drive, int steer)
/* Print txt and drive motors and encoders for 1.5s */
{ LCDPrintf("%s\n", txt);
  MOTORDrive(1, drive);
  SERVOSet  (1, steer);
  OSWait(1500);
//  LCDPrintf("Enc. %5d\n", ENCODERRead(1));
}

 
int main ()
{ LCDPrintf("Ackermann Steer\n");
	
  Mdrive("Forward",     60, 127);
  Mdrive("Backward",   -60, 127);
  Mdrive("Left Curve",  60,   0);
  Mdrive("Right Curve", 60, 255);
  Mdrive("Stop",         0,  0);

  return 0;
}

