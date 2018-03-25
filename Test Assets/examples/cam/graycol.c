// EyeBot Demo Program: Camera Interactive, T. Bräunl, Nov 2015
#include "eyebot.h"

int main()
{ BYTE img[QVGA_SIZE];
  int k, size = 0, gray = 1;
  
  LCDMenu("SIZE", "COL", "", "END");
  CAMInit(QQVGA);  // automatically sets LCDIMageSize and IPSize

  do 
  { k = KEYRead();
    if (k==KEY1)
    { size = !size;
      LCDClear();
      if (size) CAMInit(QVGA);
        else    CAMInit(QQVGA);
    }
    else if (k==KEY2) {
        gray = !gray;
    }

    if (gray)
    { CAMGetGray(img);
      LCDImageGray(img);
        LCDMenu("SIZE", "COL", "", "END");
    }
    else  // color
    { CAMGet(img);
      LCDImage(img);
        LCDMenu("SIZE", "COL", "", "END");
    }
  } while  (k != KEY4);

  return 0;
  
}
