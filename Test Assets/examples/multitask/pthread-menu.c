#include "eyebot.h"
#include <pthread.h>
#include <stdio.h>
int enable=0, running=1, count=0;

void *slave(void *arg)
{ while(running) if (enable)
  { LCDPrintf("%d", (int) arg);
    count++;
  }
  return NULL;
}

void *master(void *arg)
{ while(running) 
  { switch (KEYRead())
    { case KEY1: enable=1; break;
      case KEY2: enable=0; break;
      case KEY4: running=0;
    } 
    if (count>1000)
    { enable=0;
        LCDClear();
        LCDMenu("RUN", "STOP", "", "END");
        count = 0;
      enable=1;
    }
  }
  return NULL;
}  


int main()
{ pthread_t t1, t2, t3;

  XInitThreads();
  LCDPrintf("Multitasking\n");
  LCDMenu("RUN", "STOP", " ", "END");
  
  pthread_create(&t1, NULL, slave, (void *) 1);
  pthread_create(&t2, NULL, slave, (void *) 2);
  pthread_create(&t3, NULL, master,(void *) 0);

  pthread_exit(0);  // will temrinate program
}
