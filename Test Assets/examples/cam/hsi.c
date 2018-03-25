// EyeBot Demo Program: Display color, gray, binary -- hue, sat, intensity
#include "eyebot.h"
#define SIZE QVGA_SIZE
#define PIX  (SIZE/3)
#define XS 160
#define YS 120
#define X   80
#define Y   60
#define D    5
#define MID (3*(Y*XS + X))


int main()
{ BYTE image[SIZE];
  BYTE h[PIX], s[PIX], i[PIX], g[PIX], b[PIX];

  CAMInit(QQVGA);
  LCDMenu(" ", " ", " ", "END");
  LCDSetPrintf(20,0, "Color, Gray, Binary - Hue, Sat Intensity");

  do
  { CAMGet(image);
    LCDImageStart(  0, 0, 160,120);
    LCDImage(image);
    IPCol2Gray(image, g);                        // make gray
    LCDImageStart(160, 0, 160,120);
    LCDImageGray(g);
    for (int i=0; i<PIX; i++) b[i] = (g[i]<127); // make bin
    LCDImageStart(320, 0, 160,120);
    LCDImageBinary(b);

    IPCol2HSI(image, h, s, i);                   // disect HSI
    LCDImageStart(  0,120, 160,120);
    LCDImageGray(h);
    LCDImageStart(160,120, 160,120);
    LCDImageGray(s);
    LCDImageStart(320,120, 160,120);
    LCDImageGray(i);
  } while (KEYRead() != KEY4);
  CAMRelease();
  return 0;
}
