#include <pthread.h>
#include <stdio.h>
#include "eyebot.h"

pthread_mutex_t  print;

void *slave(void *arg)
{ for (int i=1; i<=9; i++)
  { pthread_mutex_lock(&print);
      LCDPrintf("START-%d -%d- ", (int) arg, i); 
      LCDPrintf("%d-stop\n", (int) arg); 
    pthread_mutex_unlock(&print);
  }
  return NULL;
}


int main()
{ pthread_t t1, t2;

  XInitThreads();
  LCDMenu("", "", "", "END");
  pthread_mutex_init(&print, NULL);
  pthread_create(&t1, NULL, slave, (void *) 1);
  pthread_create(&t2, NULL, slave, (void *) 2);

  KEYWait(KEY4);
  pthread_exit(0); // will terminate program
}
