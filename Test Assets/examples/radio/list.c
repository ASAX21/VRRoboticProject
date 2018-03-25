#include "eyebot.h"

int main ()
{ int l, i;
  int id[256];
  int myid;

  LCDMenu("", "", "", "END");
  LCDPrintf("init\n");
  RADIOInit();

  myid = RADIOGetID();
  LCDPrintf("my id %d\n", myid);

  LCDPrintf("scanning\n");
  l = RADIOStatus(id); 

  LCDPrintf("Other robots: %d\n", l);
  for (i=0; i<l; i++)
    LCDPrintf("%d  ", id[i]);
  LCDPrintf("\n");
  KEYWait(KEY4);
}

