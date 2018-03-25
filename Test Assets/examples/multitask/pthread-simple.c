#include <pthread.h>
#include <stdio.h>
#include "eyebot.h"

void *slave(void *arg)
{ for (int i=1; i<=9; i++)
  { LCDPrintf("START-%d -%d- ", (int) arg, i); 
    LCDPrintf("%d-stop\n", (int) arg); 
  }
  return NULL;
}


int main()
{ pthread_t t1, t2;

  XInitThreads();
  LCDMenu("", "", "", "END");

  pthread_create(&t1, NULL, slave, (void *) 1);
  pthread_create(&t2, NULL, slave, (void *) 2);

  KEYWait(KEY4);
  pthread_exit(0); // will terminate program
}
