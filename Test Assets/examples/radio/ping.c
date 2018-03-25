// Ping-Pong radio communication program
// T. Braunl, May 2017

#include "eyebot.h"
#define MAX 10

int main ()
{ int  k, p, i, num, ret;
  int  id[256];
  int  myid, partnerid;
  char buf[MAX] = "00000";

  LCDMenu("MASTER", "SLAVE", "", "END");
  RADIOInit();
  myid = RADIOGetID();
  LCDPrintf("my id %d\n", myid);

  k = KEYGet();
  if (k==KEY4) return 0; // exit
  if (k==KEY1) // master only
  { LCDPrintf("scanning (takes time ...)\n");
    ret = RADIOStatus(id); 
    LCDPrintf("Total of %d robots: ");
    for (i=0; i<ret; i++) LCDPrintf(" %d", id[i]);
    LCDPrintf("\n");

    // select partner robot
    if (ret<2) {LCDPrintf("Not enough partners - Bye\n"); return 0;}
    p=0;
    partnerid = id[p];
    if (myid==id[p]) {p++; partnerid = id[p];}
    LCDPrintf("partner is %d\n", partnerid);

    LCDPrintf("I will start\n");
    RADIOSend(partnerid, buf);
  }
  LCDPrintf("I am waiting for partner\n");

  for (i=0; i<10; i++)
  { RADIOReceive(&partnerid, buf, MAX);
    LCDPrintf("received from %d text %s\n", partnerid, buf);

    sscanf (buf, "%d", &num);
    num++;
    sprintf(buf, "%05d", num);
    RADIOSend(partnerid, buf);
  }

  KEYWait(KEY4);
}

