// EyeBot Demo Program: Drive, T. Bräunl, Nov. 2015
#include "eyebot.h"

int main()
{ int x, y, phi;

  VWDrive(200,  50,  100); VWWait(); // deviate 50 from straight
  VWGetPosition(&x, &y, &phi);
  LCDPrintf("xy200,50 pos %d %d ori %d\n");
  
  VWDrive(200,-200,  100); VWWait(); //  deviate back 200 
  VWGetPosition(&x, &y, &phi);
  LCDPrintf("xy200,-200 pos %d %d ori %d\n");
  
  VWDrive(200,   0,  100); VWWait(); //  deviate back 200 
  VWGetPosition(&x, &y, &phi);
  LCDPrintf("xy200,0 pos %d %d ori %d\n");
}
