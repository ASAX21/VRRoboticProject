// EyeBot Demo Program: Image File I/O, T. Bräunl, June 2015
#include "eyebot.h"

int main()
{ BYTE img[QVGA_SIZE];
  
  IPSetSize(QVGA);
  LCDImageStart(0,10, 320,240);

  LCDSetPrintf(0,0, "IMAGE 1: Color");
  IPReadFile("pic/image1.ppm", img);
  LCDImage(img);
  OSWait(2000); // 2s
  
  LCDSetPrintf(0,0, "IMAGE 2: Gray");
  IPReadFile("pic/image2.pgm", img);
  LCDImageGray(img);
  OSWait(2000); // 2s
  
  LCDSetPrintf(0,0, "IMAGE 3: Binary");
  IPReadFile("pic/image3.pbm", img);
  LCDImageGray(img); // same function as Gray pgm
  OSWait(2000); // 2s  
}
