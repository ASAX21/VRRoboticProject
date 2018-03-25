/*------------------------------------------------------------------------
| Filename:     speed.c
| Author:       Thomas Braunl UWA 2017
| Description:  Drive using SetSpeed functions
-------------------------------------------------------------------------- */

#include "eyebot.h"
#include <stdio.h>
#include <math.h>

int main ()
{ int k, x,y,phi;
  LCDMenu("GO","BACK","CIRCLE","END");

  do
  { switch(k = KEYGet())
    { case KEY1: VWSetSpeed(+300,  0); break;
      case KEY2: VWSetSpeed(-300,  0); break;
      case KEY3: VWSetSpeed(+300, 90); break;
    }
    OSWait(1000);    // 1 sec 
    VWSetSpeed(0,0); // stop

    VWGetPosition(&x, &y, &phi);
    LCDPrintf("x=%d y=%d p=%d\n", x,y,phi);
  } while(k != KEY4);
}
