#include "eyebot.h"
#define SAFE 300
#define PSD_LEFT  1
#define PSD_FRONT 2
#define PSD_RIGHT 3

int main ()
{ BYTE img[QVGA_SIZE];
  int dir, l, f, r;

  LCDMenu("", "", "", "END");
  CAMInit(QVGA);

  while(KEYRead() != KEY4)
  { CAMGet(img);    // demo
    LCDImage(img);  // only
    l = PSDGet(PSD_LEFT);
    f = PSDGet(PSD_FRONT);
    r = PSDGet(PSD_RIGHT);
    LCDSetPrintf(18,0, "PSD L%3d F%3d R%3d", l, f, r);
    if (l>SAFE && f>SAFE && r>SAFE)
//  if (l>SAFE && f>SAFE && r>SAFE && !VWStalled())
      VWStraight( 100, 200); // 100mm at 10mm/s
    else
    { VWStraight(-25, 50);   // back up
      VWWait();
      dir = (((float)rand()/(float)RAND_MAX - 0.5))*180;
      LCDSetPrintf(19,0, "Turn %d", dir);
      VWTurn(dir, 45);      // turn random angle
      VWWait();
      LCDSetPrintf(19,0, "          ");
    }
    OSWait(100);
  } // while
  return 0;
}
