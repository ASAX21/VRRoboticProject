// EyeBot Demo Program: Image File I/O, T. Bräunl, June 2015
#include "eyebot.h"

int main()
{ BYTE img[QVGA_SIZE];
  int pos;
  
  for (int i=0; i<50; i++)
   for (int j=0; j<320; j++)
   { pos = 3*(320*i + j);
     img[pos]=0; img[pos+1]=0; img[pos+2]=0;
   }
   
  for (int i=50; i<100; i++)
   for (int j=0; j<320; j++)
   { pos = 3*(320*i + j);
     img[pos]=255; img[pos+1]=0; img[pos+2]=0;
   }


  for (int i=100; i<150; i++)
   for (int j=0; j<320; j++)
   { pos = 3*(320*i + j);
     img[pos]=0; img[pos+1]=255; img[pos+2]=0;
   }

  for (int i=150; i<200; i++)
   for (int j=0; j<320; j++)
   { pos = 3*(320*i + j);
     img[pos]=0; img[pos+1]=0; img[pos+2]=255;
   }

  for (int i=200; i<240; i++)
   for (int j=0; j<320; j++)
   { pos = 3*(320*i + j);
     img[pos]=255; img[pos+1]=255; img[pos+2]=255;
   }

  IPSetSize(QVGA);
  IPWriteFile("pic/rgb.ppm", img);
  LCDImageStart(0,0, 320,240);
  LCDImage(img);
  LCDPrintf("Black - Red - Green - Blue - White");
  OSWait(5000); // 5s
  }
