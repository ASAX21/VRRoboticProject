// EyeBot Demo Program: Soft-Button Test, T. Bräunl, June 2015
#include "eyebot.h"

int main()
{ int i,k;

  LCDMenu("1","2","3","4");

  LCDPrintf("KEY Get\n");
  for (i=0; i<4; i++)
  { LCDPrintf("press key (get)\n");
    k = KEYGet();
    LCDPrintf("key presed: %d\n", k);
  }

  LCDPrintf("\nKEY Read\n");
  for (i=0; i<2; i++)
  { LCDPrintf("press key (read)\n");
    do { k= KEYRead(); } while (!k);
    LCDPrintf("key pressed: %d\n", k);
  }

  LCDMenu(" "," "," ","END");
  LCDPrintf("\nKEY Wait\n");
  KEYWait(KEY4);
  return 0;
}
