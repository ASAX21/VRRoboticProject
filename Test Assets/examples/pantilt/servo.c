/* *********************************** */
/* Testing servo-mounted camera        */
/* Thomas Braunl, Jan. 2005            */
/* *********************************** */
#include "eyebot.h"

void checkpos(int pan, int tilt)
{ BYTE img[QVGA_SIZE];

  SERVOSet(1, pan);
  SERVOSet(2, tilt);
  LCDSetPrintf(0,0,"PanTlt: %3d %3d\n", pan, tilt);
  CAMGet(img);
  LCDImage(img);
}


int main()
{ int pos;

  CAMInit(QVGA);

  for (pos=128; pos<255; pos++) checkpos(pos, 128);
  for (pos=255; pos>0;   pos--) checkpos(pos, 128);
  for (pos=0;   pos<128; pos++) checkpos(pos, 128);

  for (pos=128; pos<255; pos++) checkpos(128, pos);
  for (pos=255; pos>0;   pos--) checkpos(128, pos);
  for (pos=0;   pos<128; pos++) checkpos(128, pos);

  return 0;
}

