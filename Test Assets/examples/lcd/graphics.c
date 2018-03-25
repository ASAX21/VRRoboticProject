#include "eyebot.h"

char *text = "The quick brown fox jumps over the lazy dog. 1234567890";
#define ColorConstNum 17

int TestLCDSetColor(COLOR fg, COLOR bg)
{
	static int ColorCount = 0;
	printf("LCDClear()\n");
	LCDClear();
	printf("LCDMenu()\n");
	LCDMenu("One", "Two", "Three", "Four");
	printf("LCDSetColor(%08x, %08x), %d/%d\n", (int)fg, (int)bg, ++ColorCount, ColorConstNum);
	LCDSetColor(fg, bg);
	printf("LCDPrintf(text)\n");
	LCDPrintf(text);
	printf("KEYWait(ANYKEY)\n\n");
	KEYWait(ANYKEY);
	return(0);
}


int main(void)
{       int x=0, y=0;
	printf("LCDMenu()\n");
	LCDMenu("One", "Two", "Three", "Four");
	printf("LCDPrintf(text)\n");
	LCDPrintf(text);
	printf("KEYWait(ANYKEY)\n\n");
	KEYWait(ANYKEY);

	printf("LCDClear()\n");
	LCDClear();
	printf("LCDMenu()\n");
	LCDMenu("One", "Two", "Three", "Four");
	printf("LCDSetPos(3,5)\n");
	LCDSetPos(3,5);
	printf("LCDPrintf(text)\n");
	LCDPrintf(text);
	printf("KEYWait(ANYKEY)\n\n");
	KEYWait(ANYKEY);

	printf("LCDClear()\n"); // Whether LCDClear() resets the Pos and Color or not?
	LCDClear();
	printf("LCDMenu()\n");
	LCDMenu("One", "Two", "Three", "Four");
	printf("LCDPrintf(text)\n");
	LCDPrintf(text);
	printf("KEYWait(ANYKEY)\n\n");
	KEYWait(ANYKEY);

	// LCDMenuI
	printf("Testing LCDMenuI() ...\n");
	printf("LCDClear()\n");
	LCDClear();
	printf("LCDMenuI()\n");
	LCDMenuI(0, "GREEN One", GREEN, BLUE);
	printf("LCDMenuI()\n");
	LCDMenuI(1, "BLUE Two", BLUE, WHITE);
	printf("LCDMenuI()\n");
	LCDMenuI(2, "WHITE Three", WHITE, RED);
	printf("LCDMenuI()\n");
	LCDMenuI(3, "RED Four", RED, GREEN);
	printf("LCDPrintf(text)\n");
	LCDPrintf(text);
	printf("KEYWait(ANYKEY)\n\n");
	KEYWait(ANYKEY);

	// LCDPixel
	printf("LCDClear()\n");
	LCDClear();
	printf("LCDMenu()\n");
	LCDMenu("One", "Two", "Three", "Four");
	printf("LCDPixel(50, 50, WHITE)\n");
	LCDPixel(50, 50, WHITE);
	printf("KEYWait(ANYKEY)\n\n");
	KEYWait(ANYKEY);

	printf("LCDLine(0, 0, 200, 100, GREEN)\n");
	LCDLine(0, 0, 200, 100, GREEN);
	printf("KEYWait(ANYKEY)\n\n");
	KEYWait(ANYKEY);

	// int fill
	printf("LCDArea(50, 50, 100, 120, BLUE, 0)\n");
	LCDArea(50, 50, 100, 120, BLUE, 0);
	printf("KEYWait(ANYKEY)\n\n");
	KEYWait(ANYKEY);

	printf("LCDArea(50, 50, 100, 120, BLUE, 1)\n");
	LCDArea(50, 50, 100, 120, BLUE, 1);
	printf("KEYWait(ANYKEY)\n\n");
	KEYWait(ANYKEY);

	// LCDCircle
	printf("LCDCircle(140, 140, 50, ORANGE, 0)\n");
	LCDCircle(140, 140, 50, ORANGE, 0);
	printf("KEYWait(ANYKEY)\n\n");
	KEYWait(ANYKEY);

	printf("LCDCircle(140, 140, 50, ORANGE, 1)\n");
	LCDCircle(140, 140, 50, ORANGE, 1);
	printf("KEYWait(ANYKEY)\n\n");
	KEYWait(ANYKEY);

	printf("KEYXY(&x, &y)\n");
	KEYGetXY(&x, &y);
	printf("Result: x=%d, y=%d\n", x, y);
	printf("KEYWait(ANYKEY)\n\n");
	KEYWait(ANYKEY);

	return 0;
}
