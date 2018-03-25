// EyeBot Demo Program: Image Motion, T. Bräunl, June 2015
#include "eyebot.h"
#define RES  QVGA
#define SIZE QVGA_SIZE

void image_diff(BYTE i1[SIZE], BYTE i2[SIZE], BYTE d[SIZE])
{ for (int i=0; i<SIZE; i++)
    d[i] = abs(i1[i] - i2[i]);
}

int avg(BYTE d[SIZE])
{ int i, sum=0;
  for (i=0; i<SIZE; i++)
    sum += d[i];
  return sum / SIZE;
}

int main()
{ BYTE image1[SIZE], image2[SIZE], diff[SIZE];
  int avg_diff, delay;

  CAMInit(RES);
  LCDMenu(" ", " ", " ", "END");
  
  while (KEYRead() != KEY4)
  { CAMGetGray(image1);
    OSWait(100);  // Wait 0.1s
    CAMGetGray(image2);
    
    image_diff(image1, image2, diff);
    LCDImageGray(diff);
    avg_diff = avg(diff);
    LCDSetPrintf(0,50, "Avg = %3d", avg_diff);

    if (avg_diff > 15)  // Alarm threshold
    { LCDSetPrintf(2,50, "ALARM!!!");
      delay = 10;
    }
    if (delay) delay--;
      else LCDSetPrintf(2,50, "        "); // clear text after delay
    OSWait(100);  // Wait 0.1s
  }  
}
