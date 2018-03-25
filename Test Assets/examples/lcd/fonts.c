#include "eyebot.h"
#include <unistd.h>

int main() {
	LCDPrintf("Standard default font\n");
	LCDSetFont(HELVETICA, NORMAL);
	LCDPrintf("Printing in Helvetica Normal\n");
	
	LCDSetFont(HELVETICA, BOLD);
	LCDPrintf("Printing in Helvetica Bold\n");

	LCDSetFont(TIMES, NORMAL);
	LCDPrintf("Printing in Times Normal\n");

	LCDSetFont(TIMES, BOLD);
	LCDPrintf("Printfing in Times Bold\n");

	LCDSetFont(COURIER, NORMAL);
	LCDPrintf("Printing in Courier Normal\n");

	LCDSetFont(COURIER, BOLD);
	LCDPrintf("Printing in Courier Bold\n");

	LCDSetFont(HELVETICA, NORMAL);
	LCDSetFontSize(14);
	LCDPrintf("Increasing font size to 14\n");

	sleep(2);
}
