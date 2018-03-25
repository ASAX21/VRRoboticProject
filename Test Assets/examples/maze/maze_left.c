/* Recursive Maze exploration program   */
/* in "MicroMouse" style                */
/*                                      */
/* Thomas Braunl, UWA, 1998, 2000, 2004 */
#include "eyebot.h"

#define SPEED    360
#define ASPEED    45
#define THRES    175
#define Left       1
#define Front      2
#define Right      3

void turn_left()
{ VWTurn(90, ASPEED);  /* turn */
  VWWait();
}
void turn_right()
{ VWTurn(-90, ASPEED);  /* turn */
  VWWait();
}
void straight()
{ VWStraight(360, SPEED);      /* go one step */
  VWWait();
}


int main ( )
{ 
  int f,l,r;
  LCDPrintf("MAZE Left\n");
  LCDMenu("","","","END");

  do
  { 
     f = PSDGet(Front) > THRES;
     l = PSDGet(Left)  > THRES; 
     r = PSDGet(Right) > THRES;
 
    // Drive left when possible, if not straight, if not right, else turn 180
    // .....

  } while (KEYRead() != KEY4);
  return 0;
}
