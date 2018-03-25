#include "eyebot.h"

int main()
{ int k, done;

  LCDPrintf("Select your sound\n");
  LCDMenu("BEEP", "MELODY", "", "END");
  do
  { switch (k = KEYGet())
    { case KEY1: AUBeep(); break;
      case KEY2: AUPlay("/home/pi/eyebot/bin/sound/welcome.wav"); break;
    }

    LCDClear();
  LCDMenu("BEEP", "MELODY", "", "END");
    do
    { OSWait(500);
      if ((done = AUDone())) LCDPrintf("done\n");
        else                 LCDPrintf("playing\n");
    } while (!done);
  } while (k != KEY4);
}

