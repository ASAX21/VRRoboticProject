// EyeBot Demo Program: Drive, T. Bräunl, Nov. 2015
#include "eyebot.h"

int main()
{ int x, y, p;
  for (int i=0; i<2; i++)  // run twice
  { VWStraight(500, 50);  // 0.5m in ca. 5s
    while (!VWDone())
    { VWGetPosition(&x, &y, &p);
      LCDPrintf("X:%4d; Y:%4d; P:%4d\n", x, y, p);
      if (PSDGet(2) < 100) VWSetSpeed(0,0);  // STOP if obstacle in front
    }
    VWTurn(180, 60);      // half turn (180 deg) in ca. 3s
    VWWait();         // wait until completed
  }
}
