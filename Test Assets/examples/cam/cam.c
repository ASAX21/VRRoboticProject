// EyeBot Demo Program: Camera Continuous Display, T. Braunl, May 2016
#include "eyebot.h"

int main()
{ BYTE img[QVGA_SIZE];

  LCDMenu("", "", "", "END");
  CAMInit(QVGA);
  do { CAMGet(img);
       LCDImage(img);
  } while (KEYRead() != KEY4);

  return 0;
}
