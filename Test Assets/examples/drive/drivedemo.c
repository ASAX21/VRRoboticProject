/*------------------------------------------------------------------------
| Filename: drive_demo.c
|
| Author:       Nicholas Tay, UWA 1997
|               Thomas Braunl UWA 2000, 2004
|
| Description:  Drives EyeBot vehicle
|		( Revised from drive_rc.c )
|
-------------------------------------------------------------------------- */

#include "eyebot.h"

#include <stdio.h>
#include <math.h>

#define functions 9

char fname[functions][32]=
    {"Forward",  "Backward", "Rotate Left", "Rotate Right",
     "Curve Left\n(FORWARD)", "Curve Right\n(FORWARD)", "Curve Left\n(BACKWARD)",
     "Curve Right\n(BACKWARD)", "SetPos [0,0,0]"};

//velocities
int vel[functions][2] =
    { { 300,  0}, {-300,   0}, {   0, 30}, {0, -30},
      { 300, 30}, { 300, -30}, {-300, 30}, 
      {-300,-30}, { 0,  0} };


int main (){
  int x, y, phi, v, w;
  int fnum = 0, done = 0;

  v = 0;  w = 0;

  do {
    LCDClear();
    LCDMenu("+", "-", "GO", "END");
    LCDPrintf("%s\n", fname[fnum]);
    VWGetPosition(&x, &y, &phi);
    LCDPrintf("x = %d   \n", x);
    LCDPrintf("y = %d   \n", y);
    LCDPrintf("p = %d   \n", phi);

    v = vel[fnum][0];
    w = vel[fnum][1];
    LCDPrintf("v=%d, w=%d\n", v, w);

    switch(KEYGet()) {
      case KEY1: fnum = (fnum+1) % functions;
                 break;
      case KEY2: fnum = (fnum-1 +functions) % functions;
                 break;
      case KEY3: if (fnum<8)
                 { VWSetSpeed(v,w);
                   LCDMenu(" ", " ", "STOP", " ");
                   KEYWait(KEY3); // continue until key pressed
                 }
                 else if (fnum==8)
                   VWSetPosition(0,0,0);
                 break;
      case KEY4: done = 1;
                 break;
    }
    VWSetSpeed(0,0); // stop
  } while (!done);

  return 0;
}
