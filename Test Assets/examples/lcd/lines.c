// EyeBot Demo Program: Text Output Test, T. Bräunl, June 2015
#include "eyebot.h"

int main()
{ LCDMenu("", "", "", "END");
  for (int i=0; i<10; i++)
  { LCDPrintf("%2d Text Text Text\n", i);
  }
  KEYWait(KEY4);
}
