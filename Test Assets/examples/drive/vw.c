/*------------------------------------------------------------------------
| Filename:     vw.c
| Author:       Thomas Braunl UWA 2017
| Description:  Drive using VW functions
-------------------------------------------------------------------------- */

#include "eyebot.h"
#include <stdio.h>
#include <math.h>

int main ()
{ int k, x,y,phi;
  LCDMenu("GO","BACK","CURVE","END");

  do
  { switch(k = KEYGet())
    { case KEY1: VWStraight(+400, 200); break;       // distance in mm; speed in mm/s
      case KEY2: VWStraight(-400, 200); break;
      case KEY3: VWCurve   (+400,  90, 200); break;  // distance, angle, speed
    }
    VWWait();  // wait until drive is completed

    VWGetPosition(&x, &y, &phi);
    LCDPrintf("x=%d y=%d p=%d\n", x,y,phi);
  } while(k != KEY4);
}
