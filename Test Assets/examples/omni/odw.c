#include <eyebot.h>

int main()
{
 	LCDPrintf("Testing\nWheelchair\nGraphics\n");
    LCDMenu("","","","END");
	while( KEYRead() != KEY4 );

	return 0;
}

