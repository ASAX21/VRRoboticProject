// EyeBot Demo Program: Drive, T. Bräunl, Nov. 2015
#include "eyebot.h"

int main()
{ LCDPrintf("Drive straight\n");
  VWStraight(1000, 200);  // 1m in ca. 5s
  VWWait();          // wait until completed

  LCDPrintf("Rotate\n");
  VWTurn(180, 60);        // half turn in ca. 3s
  VWWait();          // wait until completed

  LCDPrintf("Drive straight\n");
  VWStraight(1000, 200);  // 1m in ca. 5s
  VWWait();          // wait until completed

  LCDPrintf("Rotate\n");
  VWTurn(180, 60);        // half turn in ca. 3s
  VWWait();          // wait until completed
}
