// EyeBot Demo Program: Text Output Test, T. Bräunl, June 2015
#include "eyebot.h"

int main()
{ LCDMenu("", "", "", "END");

  LCDSetColor(RED, BLACK);
  for (int i=0; i<2; i++) LCDPrintf("%2d RED RED RED\n", i);
  LCDSetColor(GREEN, BLACK);
  for (int i=0; i<2; i++) LCDPrintf("%2d GREEN GREEN GREEN\n", i);
  LCDSetColor(BLUE, BLACK);
  for (int i=0; i<2; i++) LCDPrintf("%2d BLUE BLUE BLUE\n", i);

  LCDSetColor(RED, WHITE);
  for (int i=0; i<2; i++) LCDPrintf("%2d RED RED RED\n", i);
  LCDSetColor(GREEN, WHITE);
  for (int i=0; i<2; i++) LCDPrintf("%2d GREEN GREEN GREEN\n", i);
  LCDSetColor(BLUE, WHITE);
  for (int i=0; i<2; i++) LCDPrintf("%2d BLUE BLUE BLUE\n", i);

  KEYWait(KEY4);
}
