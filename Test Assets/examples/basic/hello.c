#include "eyebot.h"
#include <stdio.h>

int main()
{ LCDPrintf("HELLO from EyeBot!   ");
  LCDMenu("DONE", "BYE", "EXIT", "OUT");
  KEYWait(ANYKEY);
  return 0;
}

