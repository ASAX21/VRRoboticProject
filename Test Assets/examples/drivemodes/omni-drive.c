/* **********************************************
| Omnidirectional Drive - Mecanum Wheels
| Example for using MOTOR and QUAD functions for
| driving a vehicle with OMNI_DRIVE steering
|
| Thomas Braunl, 2004, 2005, 2017
| ---------------------------------------------- */ 
#include "eyebot.h"

void Mdrive(char* txt, int Fleft, int Fright, int Bleft, int Bright)
/* Print txt and drive motors and encoders for 1.5s */
{ LCDPrintf("%s\n", txt);
  MOTORDrive(1, Fleft);
  MOTORDrive(2, Fright);
  MOTORDrive(3, Bleft);
  MOTORDrive(4, Bright);
  OSWait(1500);
  LCDPrintf("Enc.%5d %5d\n    %5d %5d\n",
    ENCODERRead(1), ENCODERRead(2), ENCODERRead(3), ENCODERRead(4));
}


int main ()
{ LCDPrintf("Mecanum Wheels\n");

  Mdrive("Forward",     60, 60, 60, 60);
  Mdrive("Backward",   -60,-60,-60,-60);
  Mdrive("Left",       -60, 60, 60,-60);
  Mdrive("Right",       60,-60,-60, 60);
  Mdrive("Left45",       0, 60, 60,  0);
  Mdrive("Right45",     60,  0,  0, 60);
  Mdrive("Turn Spot L",-60, 60,-60, 60);
  Mdrive("Turn Spot R", 60,-60, 60,-60);
  Mdrive("Stop",         0,  0,  0,  0);
  
  return 0;
}

