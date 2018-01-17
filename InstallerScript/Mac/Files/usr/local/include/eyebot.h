/**
 * \file    eyebot.h
 * \brief   Header file for the EyeBot 7 functions
 * \author  Marcus Pham, Thomas Braunl, June 2016
 */

/*! \mainpage EyeBot 7 API
 *
 * \section Introduction
 *
 * An API to desribe the available functions on the EyeBot 7.
 * This enables users to control hardware components connected to the IO board like:
 *      - Motors
 *      - Psds
 *      - Servos
 *      - Encoders
 *      - Digital IO
 *      - Magnet
 *
 * The EyeBot 7 API also contains functions for controlling Raspberry Pi specific functions:
 *      - LCD
 *      - LCD Touch-screen and
 *      - Camera  Lists
 *      - IRTV remote
 *      - Serial Communication
 *      - Audio Functions
 *
 * As well as some core functionality:
 *      - Lists (on the LCD)
 *      - Multitasking
 *      - Timers
 *
 * All configurations are able to be set inside the hdt file (hardware description table)
 * (eg. no motors, tables, SSID, drive, USB serial params)
 * default location: /home/pi/eyebot/bin/hdt.txt
 *
 * You can also access this page when connected to the EyeBot via its IP Address
 *
 * \subsection Usage
 *
 * Programs requiring the usage of the functions defined here must:
 * 1.   Use the header command: #include "eyebot.h"
 * 2.   Compile the program with command line: gccarm (takes place of gcc compiler)
 *      The usual arguments to the compiler are also available
 *
 * \subsection Credits
 * The EyeBot 7 was designed, created and programmed at the University of Western Australia by:
 *
 * Director:    Professor Thomas Braunl
 *
 * Advisor:     Ivan Neubronner
 *
 * Hardware:    Franco Hidalgo (2015) & Thomas Smith (2014)
 *
 * Software:    Marcus Pham (2015) & Stuart Howard (2014)
 *
 * Additional Credit:
 *              Remi Keat (2013), Ke Feng - Image Processing (2015), Scott Mon (2015)
 */

#ifndef EYEBOT_H_
#define EYEBOT_H_

#include "types.h"

/** \name LCD
 * Available functions for the LCD
 */
///@{
/**
 * \brief   Print formatted string to LCD and refresh LCD. Cursor position is updated.
 * \param   const char* format : Formatted string
 * \return  int retVal : always 0
 */
int LCDPrintf(const char *format, ...);

/**
 * \brief   LCDPrintf with text position specified.
 * \param   int row : Cursor position
 * \param   int column : Cursor position
 * \param   const char* format : Formatted string
 * \return  int retVal : always 0
 */
int LCDSetPrintf(int row, int column, const char *format, ...);

/**
 * \brief   Clear the LCD display and all display buffers ie. turn it black
 * \return  int retVal : always 0
 */
int LCDClear(void);

/**
 * \brief   Set the text cursor position to (row, column).
 * \param   int row : Text cursor row index
 * \param   int column : Text cursor column index
 * \return  int retVal : always 0
 */
int LCDSetPos(int row, int column);

/**
 * \brief   Get the text cursor position.
 * \param   int* row : Pointer to cursor row index
 * \param   int* column : Pointer to cursor column index
 * \return  int retVal : always 0
 */
int LCDGetPos(int *row, int *column);

/**
 * \brief   Set the default color for text (including background).
 * \param   fg : Default color for text
 * \param   bg : Default color for text background
 * \return  0 always
 */
int LCDSetColor(COLOR fg, COLOR bg);

/**
 * \brief   Sets the font using font type and variation
 * \param   int font: The type of font (HELVETICA/TIMES/COURIER)
 * \param   int variation: The variation (NORMAL/BOLD/ITALICS)
 * \return  0 always
 */
int LCDSetFont(int font, int variation);

/**
 * \brief   Sets the font size
 * \param   int fontsize: The size of subsequent printf (8, 12, 14, 18, 24)
 * \return  0 on success, 1 for invalid fontsize
 */
int LCDSetFontSize(int fontsize);

/**
 * \brief   Update the internal mode flag bits.
 * \param	int mode : LCD Mode flag (default = 0)
 * \return  int retVal : always 0
 */
int LCDSetMode(int mode);

/**
 * \brief   Set menu entries in KEY_CLASSIC mode (4-buttons).
 * \param   char* string1 :  Menu entry for KEY1 in classic mode
 * \param   char* string2 :  Menu entry for KEY2 in classic mode
 * \param   char* string3 :  Menu entry for KEY3 in classic mode
 * \param   char* string4 :  Menu entry for KEY4 in classic mode
 * \return  int retVal : always 0
 */
int LCDMenu(char *st1, char *st2, char *st3, char *st4);

/**
 * \brief   Set specific menu entry in KEY_CLASSIC mode (index given by pos).
 * \param   int pos : Select menu entry in classic mode (from 1->4)
 * \param   char* string : Menu entry for the key at specified index
 * \param   XColor fgcol : Textcolor for the menu
 * \param   XColor bgcol : Background color for the menu
 * \return  int retVal : always 0
 */
int LCDMenuI(int pos, char *string, COLOR fg, COLOR bg);

/**
 * \brief		Get LCD resolution in pixels
 * \param       x : Where to store the width of the LCD
 * \param       y : Where to store the height of the LCD
 * \return      int retVal : always 0
 */
int LCDGetSize(int *x, int *y);

/**
 * \brief   Sets the color of the pixel at (x,y) coordinate to color.
 * \param   int x : X-coordinate of the pixel
 * \param   int y : Y-coordinate of the pixel
 * \param   col : RGB color value for the pixel
 * \return  int retVal : always 0
 */
int LCDPixel(int x, int y, COLOR col);

/**
 * \brief   Get the RGB color value of the pixel at (x,y) coordinate.
 * \param   int x : X-coordinate of the pixel
 * \param   int y : Y-coordinate of the pixel
 * \return  COLOR color : RGB color value
 */
COLOR LCDGetPixel (int x, int y);

/**
 * \brief   Draw a color line from (x1,y1) to (x2,y2).
 * \param   int x1 : X-coordinate of first pixel
 * \param   int y1 : Y-coordinate of first pixel
 * \param   int x2 : X-coordinate of second pixel
 * \param   int y2 : Y-coordinate of second pixel
 * \param   COLOR color :  RGB color value for the pixel
 * \return  int retVal : always 0
 */
int LCDLine(int x1, int y1, int x2, int y2, COLOR col);

/**
 * \brief   Draw a color-filled rectangle with (x1,y1) as top-left coordinate and (x2,y2) as the bottom-right coordinate.
 * \param   int x1 : X-coordinate of top-left pixel
 * \param   int y1 : Y-coordinate of top-left pixel
 * \param   int x2 : X-coordinate of bottom-right pixel
 * \param   int y2 : Y-coordinate of bottom-right pixel
 * \param   COLOR col : RGB fill color value
 * \return  int retVal : always 0
 */
int LCDArea(int x1, int y1, int x2, int y2, COLOR col, int fill);

/**
 * \brief   Draws a circle
 * \param   int x1 : X-coordinate of the centre
 * \param   int y1 : Y-coordinate of the centre
 * \param   int size : radius of the circle
 * \param   COLOR col: the color of the circle
 * \param   int fill : fill(1) or no fill(0)
 * \return  int retVal : always 0
 */
int LCDCircle(int x1, int y1, int size, COLOR col, int fill);

/**
 * \brief   Specifies the size of subsequent LCDImage functions
 * \param   int type: the size of the image (QQVGA, QVGA, VGA, CAM1MP, CAMHD, CAM5MP, CUSTOM), default QVGA
 * \return  int ret: always 0
 */
int LCDImageSize(int t);

/**
 * \brief		Sets the location and size parameters for subsequent image draws
 * \param		int t : the image type
 * \param       int x : x-coordinate of top-left image position
 * \param       int y : y-coordinate of top-left image position
 * \param       int xs : Image width
 * \param       int ys : Image height
 * \return      int retVal : always 0
 */
int LCDImageStart(int x, int y, int xs, int ys);

/**
 * \brief		Draws a colour (RGB) image to the LCD
 * \param       img : A pointer to the BYTE buffer of the image
 * \return      int retVal : always 0
 */
int LCDImage(BYTE *img);

/**
 * \brief		Draws a greyscale [0..255] image to the LCD
 * \param       img : A pointer to the BYTE buffer of the image
 * \return      int retVal : always 0
 */
int LCDImageGray(BYTE *g);

/**
 * \brief		Draws a binary [0..1] image to the LCD
 * \param       img : A pointer to the BYTE buffer of the image
 * \return      int retVal : always 0
 */
int LCDImageBinary(BYTE *b);

/**
 * \brief  Refresh the screen. Refreshes the menu
 * \return    int retVal : always 0
 */
int LCDRefresh(void);
///@}

/******************************************************************************************************************************/

/** \name KEY
 * Available functions for usage of the Keys
 */
///@{
/**
 * \brief	Blocking read (and wait) for key press (returns KEY1..KEY4)
 * \return  (int) KEYCODE retKey : Keycode value
 */
int KEYGet(void);

/**
 * \brief	Non-blocking read of key press (returns NOKEY=0 if no key)
 * \return  (int) KEYCODE retKey : Keycode value
 */
int KEYRead(void);

/**
 * \brief	Wait for specific keys to be pressed. (use ANYKEY for any key)
 * \param	int key : Expected keycode values (KEY1..KEY4)
 * \return  (int) KEYCODE retKey : Keycode value
 */
int KEYWait(int key);

/**
 * \brief	Blocking read for touch at any position, and returns the coordinates of the pressed position
 * \return  0 always
 */
int KEYGetXY (int *x, int *y);  

/**
 * \brief	Non-blocking read for touch at any position, returns coordinates of pressed position
 * \return  0 on a keypress, 1 otherwise
 */
int KEYReadXY(int *x, int *y);

///@}

/******************************************************************************************************************************/

/** \name CAM
 * Available functions for usage of the Camera
 */
///@{
/**
 *  \brief  Initialises the camera.
 *  \param  int resolution The resolution of the camera (QQVGA, QVGA, VGA, CAM1MP, CAMHD, CAM5MP, CUSTOM) default QVGA
 *  \return Nonzero on success, zero on failure.
 */
int CAMInit(int resolution);

/**
 *  \brief Stops and releases all camera structures.
 *  \return Nonzero on success, zero on failure.
 *  \details Releases all allocated memory and stops video capture.
 */
int CAMRelease(void);

/**
 *  \brief Retrieve a camera frame into the provided buffer.
 *  \param BYTE *buf: the buffer to store the image
 *  \return Nonzero on success, zero on failure.
 */
int CAMGet(BYTE *buf);

/**
 * \brief Grabs a gray image off the camera
 * \param the byte stream to store the image data, data in Gray format
 * \return 0 iff initialised correctly, otherwise 1
 */
int CAMGetGray(BYTE *buf);

///@}

/******************************************************************************************************************************/

/** \name IP
 * Available functions for image processing
 */
///@{
/**
 * \brief Sets the size for the image processing (also automatically set by CAMInit).
 * \param mode the resolution (QQVGA, QVGA, VGA, CAM1MP, CAMHD, CAM5MP, CUSTOM) specifying the resolution of the camera
 * \return 0 if initialised correctly, otherwise 1
 */
int   IPSetSize(int resolution);

/**
 * \brief Read PNM file, fill/crop if req.; return 0 for color, 1 for gray image
 * \param filename the file to open
 * \param img the buffer to store the image
 * \return Returns: 1 =PBM, 2 =PGM, 3 =PPM (or 4, 5, 6 for binary versions)
 */
int   IPReadFile(char *filename, BYTE* img);

/**
 * \brief Write color PNM file
 * \param filename the file to save to
 * \param img the buffer to store the image
 * \return 0 always
 */
int   IPWriteFile(char *filename, BYTE* img);

/**
 * \brief Write greyscale PNM file
 * \param filename the file to save to
 * \param img the buffer to store the image
 * \return 0 always
 */
int   IPWriteFileGray(char *filename, BYTE* gray);

/**
 * \brief apply Laplace tranform to the image
 * \param grayIn the input image
 * \param grayOut the formatted output image
 */
void  IPLaplace(BYTE* grayIn, BYTE* grayOut);

/**
 * \brief apply Sobel tranform to the image
 * \param imageIn the input image
 * \param imageOut the formatted output image
 */
void  IPSobel(BYTE* imageIn, BYTE* imageOut);

/**
 * \brief Transfer color to gray image
 * \param imgIn the input image
 * \param grayOut the formatted output image
 */
void  IPCol2Gray(BYTE* imgIn, BYTE* grayOut);

/**
 * \brief Transfer grey to color (3-byte) image
 * \param imgIn the input image
 * \param colOut the formatted output image
 */
void  IPGray2Col(BYTE* imgIn, BYTE* colOut);

/**
 * \brief Transfer grey to color (3-byte) image
 * \param imgIn the input image
 * \param colOut the formatted output image
 */
void  IPRGB2Col (BYTE* r, BYTE* g, BYTE* b, BYTE* imgOut);

/**
 * \brief Transfer color image to hue sat int
 * \param img the input image
 * \param h the hue
 * \param s the saturation
 * \param i the intensity
 */
void  IPCol2HSI (BYTE* img, BYTE* h, BYTE* s, BYTE* i);

/**
 * \brief Overlay c2 onto c1, all color images
 * \param c2
 * \param c1
 * \param cOut the output image
 */
void  IPOverlay(BYTE* c1, BYTE* c2, BYTE* cOut);

/**
 * \brief Overlay gray image g2 onto g1, using col
 * \param g1
 * \param g2
 * \param col
 * \param cOut the output image
 */
void  IPOverlayGray(BYTE* g1, BYTE* g2, COLOR col, BYTE* cOut);

/**
 * \brief Transforms RGB to a COLOR (for one Pixel)
 * \param r the red
 * \param g the green
 * \param b the blue
 * \return COLOR the COLOR
 */
COLOR IPPRGB2Col(BYTE r, BYTE g, BYTE b);

/**
 * \brief Transforms COLOR to RGB (for one Pixel)
 * \param r the red
 * \param g the green
 * \param b the blue
 * \param COLOR the COLOR
 */
void  IPPCol2RGB(COLOR col, BYTE* r, BYTE* g, BYTE* b);

/**
 * \brief Transforms RGB to HSI (for one pixel)
 * \param COLOR c the color to transform
 * \param h the hue
 * \param s the saturation
 * \param i the intensity
 */
void  IPPCol2HSI(COLOR c, BYTE* h, BYTE* s, BYTE* i);

/**
 * \brief Convert RGB to hue (0 for gray values)
 * \param r the red
 * \param g the green
 * \param b the blue
 * \return BYTE a hue value
 */
BYTE  IPPRGB2Hue(BYTE r, BYTE g, BYTE b);

/**
 * \brief Convert RGB to hue, sat, int; hue=0 for gray values
 * \param r the red
 * \param g the green
 * \param b the blue
 * \param h the hue
 * \param s the saturation
 * \param i the intensity
 */
void  IPPRGB2HSI(BYTE r, BYTE g, BYTE b, BYTE* h, BYTE* s, BYTE* i);

///@}

/******************************************************************************************************************************/

/** \name OS
 * Available functions for obtaining system information
 */
///@{
/**
 * changed - return a local variable? out parameter is buffer to store string
 * \brief       Executes a system command and obtains the string returned after the command
 * \param       command the command to run
 * \return      the string returned after execution
 */
int OSExecute(char* command, char* out);

/**
 * \brief       Returns string containing running RoBIOS version.
 * \return      the RoBIOS version
 */
int OSVersion(char* buf);

/**
 * \brief Gives the version of the IO board
 * \param version, the string to store the version string
 * \return 0 on success, 1 on failure
 */
int OSVersionIO(char* buf);

/**
 * \brief     Inform the user how fast the processor runs.
 * \return		the actual clockrate of CPU in Hz
 */
int OSMachineSpeed(void);

/**
 * \brief     Inform the user in which environment the program runs.
 * \return	  the Type of used hardware
 * \details   Valid values are: VEHICLE, PLATFORM, WALKER
 */
int OSMachineType(void);

/**
 * \brief       Inform the user with which name the Eyebot is titled
 * \return      the Name of actual Eyebot
 */
int OSMachineName(char* buf);

/**
 * \brief		Inform the user with which ID the Eyebot is titled
 * \return      the ID of actual Eyebot
 */
int OSMachineID(void);

///@}

/******************************************************************************************************************************/

/**
 * \brief Waits for 1 ms
 * \param n the number of ms to wait for
 * \return 0 always
 */
int   OSWait(int n);

/**
 * \brief adds a function to a 1kHz/scale timer, ie. executes every 1ms*scale
 * \detail usage: OSAttachTimer(<scale>, &<function>);
 * \param scale the rate to run the function
 * \param fct the function to run, note must return void and have no arguments
 * \return the Timer struct for the timer
 */
TIMER OSAttachTimer(int scale, void (*fct)(void));

/**
 * \brief removes the Timer from the list
 * \param handle the timer to remove
 * \return 0 always
 */
int   OSDetachTimer(TIMER t);

/**
 * \brief sets the system time
 * \param hrs the hours
 * \param mins the mins
 * \param secs the secs
 * \return 0 always
 */
int OSSetTime(int hrs,int mins,int secs);

/**
 * \brief Get system time (ticks in 1/100 sec)
 * \param hrs the pointer to the hours integer
 * \param mins the pointer to the mins integer
 * \param secs the pointer to the secs integer
 * \param ticks the pointer to the ticks integer
 * \return 0 always
 */
int OSGetTime(int *hrs,int *mins,int *secs,int *ticks);

/**
 * \brief gives you the count in 1/100 sec since the system start
 * \return the count
 */
int OSGetCount(void);

///@}

/******************************************************************************************************************************/

/** \name SER
 * Available functions for usage of the serial communication
 */
///@{
/**
 * \brief   Initialises the port
 * \param   interface the inteface type
 * \param   baud      the baud rate
 * \param   handshake  the handshake type
 * \return  0 on success, 1 otherwise
 */
int  SERInit(int interface, int baud,int handshake);

/**
 * \brief   Sends a single character to the given port handle
 * \param   interface the inteface type
 * \param   ch      the character to send
 * \return  0 on success, 1 otherwise
 */
int  SERSendChar(int interface, char ch);

/**
 * \brief   Sends a string to the given port handle
 * \param   interface the inteface type
 * \param   buf      the string to send
 * \param   size     the length of the string to send
 * \return  0 on success, 1 otherwise
 */
int  SERSend(int interface, char *buf, uint32_t size);

/**
 * \brief   Receives a single character from the given port handle
 * \param   interface the inteface type
 * \return  the character received
 */
char SERReceiveChar(int interface);

/**
 * \brief   Receives a string from port handle
 * \param   interface the inteface type
 * \param   buf      the string to store the received bytes
 * \param   size     the length of the string to receive
 * \return  n       the length of the received string
 */
int  SERReceive(int interface, unsigned char *buf);

/**
 * \brief   checks and flushes the port by sending enter until seeing prompt
 * \param   interface the inteface type
 * \return  0 on success, 1 otherwise
 */
bool SERCheck(int interface);

/**
 * \brief   flushes the port
 * \param   interface the inteface type
 * \return  0 on success, 1 otherwise
 */
int  SERFlush(int interface);

/**
 * \brief   closes the port
 * \param   interface the inteface type
 * \return  0 on success, 1 otherwise
 */
int  SERClose(int interface);

///@}

/******************************************************************************************************************************/

/** \name AU
 * Available functions for usage of the audio on the eyebot
 */
///@{
/**
 * \brief   Play beep sound
 * \return  0 always
 */
int AUBeep(void);

/**
 * \brief   Play audio sample
 * \param   sample the raw audio file
 * \return  0 always
 */
int AUPlay(char* sample);

/**
 * \brief   Non-blocking test if playing has finished

 */
int AUDone();

int AUMicrophone(void);

///@}

/******************************************************************************************************************************/

/** \name PSD
 * Available functions for access of the IR PSDs
 */
///@{
/**
 * \brief   Delivers actual timestamp or distance measured by the selected PSD.
 * \param   psd the number of the psd to read
 * \return  the actual distance in mm (converted through internal table in hdt.txt)
 */
int PSDGet(int psd);

/**
 * \brief   Delivers raw-data measured by the selected PSD
 * \param   psd the psd to read [1..6]
 * \return  the raw value
 */
int PSDGetRaw(int psd);

///@}

/******************************************************************************************************************************/

/** \name SERVO
 * Available functions for usage of the Servos
 */
///@{
/**
 * \brief   Set the given servos to the same given angle depending on table value
 * \param   servo the servo number [1..14]
 * \param   angle the angle to set it to [1..255]
 * \return  0 on success, 1 on failure
 */
int SERVOSet(int servo, int angle);

/**
 * \brief   Set the given servos to the same given angle in raw mode
 * \param   servo the servo to set [1..14]
 * \param   int angle : valid values = 0-255
 * \return  0 on success, 1 otherwise
 */
int SERVOSetRaw (int servo, int angle);

/**
 * \brief   Sets the range for the servos
 * \param servo the servo to set [1..14] limits in 1/100 sec
 * \param low the lower limit
 * \param high the upper limit
 * \return  0 on success, 1 otherwise
 */
int SERVORange(int servo, int low, int high);

///@}

/******************************************************************************************************************************/

/** \name MOTOR
 * Available functions for usage of the Motors
 */
///@{
/**
 * \brief   Set the given motors to the same given speed dependant on hdt table
 * \param   motor the motor to set [1..4]
 * \param   speed : motor speed in percent  [-100 ..+100]
 * \return  0 on success, 1 on failure
 */
int MOTORDrive(int motor, int speed);

/**
 * \brief   Set the given motor to a given speed
 * \param   motor the motor to set [1..4]
 * \param   speed : motor speed [-100 ..+100]
 * \return  0 on success, 1 on failure
 */
int MOTORDriveRaw(int motor, int speed);

/**
 * \brief   Set the given motors with PID control
 * \param   motor the motor to set [1..4]
 * \param   p the p setting [1..255]
 * \param   i the i setting [1..255]
 * \param   d the d setting [1..255]
 * \return  0 on success, 1 on failure
 */
int MOTORPID(int motor, int p, int i, int d);

/**
 * \brief   Shuts off PID control
 * \param   motor the motor to set [1..4]
 * \return  0 on success, 1 on failure
 */
int MOTORPIDOff(int motor);

/**
 * \brief   Sets the controlled motor speed in ticks/sec
 * \param   motor the motor to set [1..4]
 * \param   ticks the number of ticks per sec
 * \return  0 on success, 1 on failure
 */
int MOTORSpeed(int motor, int ticks);

///@}

/******************************************************************************************************************************/

/** \name ENCODER
 * Available functions for usage of the encoders
 */
///@{
/**
 * \brief   Reads the value from the encoder
 * \param   quad the encoder to read from  [1..4]
 * \return  the encoder value on success, 1 on failure
 */
int ENCODERRead(int quad);

/**
 * \brief   Resets the encoder to 0
 * \param   quad the encoder to reset [1..4]
 * \return  0 on success, 1 on failure
 */
int ENCODERReset(int quad);

///@}

/******************************************************************************************************************************/

/** \name VW
 * Available functions for usage of VW drive functions
 */
///@{
/**
 * \brief		Sets the VW speed, angle
 * \param       linSpeed the straightline speed (mm/s)
 * \param       angSpeed the angular turning speed (100 * rad/s)
 * \return      0 on success, 1 othewrise
 */
int VWSetSpeed(int linSpeed, int angSpeed);

/**
 * \brief		Obtains the current speeds
 * \param       linspeed the integer to store the linear speed (mm/s)
 * \param       angSpeed the integer to store the angular speed  (100 * rad/s)
 * \return      0 on success, 1 othewrise
 */
int VWGetSpeed(int *linSspeed, int *angSpeed);

/**
 * \brief		Sets the position wrt current position
 * \param       x the horizontal component (mm)
 * \param       y the vertical component (mm)
 * \param       phi the turn angle (rad*100)
 * \return      0 on success, 1 othewrise
 */
int VWSetPosition(int x, int y, int phi);

/**
 * \brief		Gets the position wrt original position
 * \param       x stores the horizontal component (mm)
 * \param       y stores the vertical component (mm)
 * \param       phi stores the turn angle (rad*100)
 * \return      0 on success, 1 othewrise
 */
int VWGetPosition(int *x, int *y, int *phi);

/**
 * \brief		Drives straight
 * \param       dist the distance to travel [mm]
 * \param       lin_speed the speed to travel at [[mm/s]
 * \return      0 on success, 1 otherwise
 */
int VWStraight(int dist, int lin_speed);

/**
 * \brief		Turns the vehicle on the spot
 * \param       angle the angle to turn to in Radians*1000 (eg. 3140 = Rotation Half Circle)
 * \param       ang_speed the speed to turn the angle [(rads/100)/s]
 * \return      0 on success, 1 otherwise
 */
int VWTurn(int angle, int ang_speed);

/**
 * \brief		Turns the vehicle in a curve
 * \param       dist the radial distance [mm]
 * \param       angle the angle to rotate in Radians * 100 (eg. 314 = half circle)
 * \param       lin_speed the speed to rotate [mm/s]
 * \return      0 on success, 1 othewrise
 */
int VWCurve(int dist, int angle, int lin_speed);

/**
 * \brief		Turns the vehicle in a curve: x[mm] straight and y[mm] left, x>|y|
 * \param       dx the distance in front of the vehicle to drive (in mm)
 * \param       dy the distance perpendicular to the direction of travel (in mm)
 * \param       lin_speed the speed to rotate
 * \return      0 on success, 1 othewrise
 */
int VWDrive(int dx, int dy, int lin_speed);

/**
 * \brief		Checks the remaining drive time
 * \return      the remaining time
 */
int VWRemain(void);

/**
 * \brief		Checks to see if the current drive is complete
 * \return      1 on completion, 0 for still driving, -1 on failure
 */
int VWDone(void);

/**
 * \brief		Blocks until the drive is complete
 * \return      0 on success, 1 othewrise
 */
int VWWait(void);

/**
 * \brief		Checks to see if the vehicle is stalled
 * \return      0 on stalled, 1 othewrise
 */
int VWStalled(void);

///@}

/******************************************************************************************************************************/

/** \name DIGITAL
 * Available functions for usage of the IO pins
 */
///@{
/**
 * \brief Sets up the digital input/output pins
 * \param io the pin to setup [1..16]
 * \param direction [i-n/o-ut/I-n pull-up/J-n pull-down]
 * \return 0 on success, 1 on failure
 */
int DIGITALSetup(int io, char direction);

/**
 * \brief Reads the digital input/output pins
 * \param io the pin to check status [1..16]
 * \return the status of the pin
 */
int DIGITALRead(int io);

/**
 * \brief Reads all digital pins
 * \return an integer representing the value current status of all pins MSB first
 */
int DIGITALReadAll(void);

/**
 * \brief Writes to the digital input/output pins
 * \param io the pin to set [1..16]
 * \param state the state of the pin [0 or 1]
 * \return 0 on success, 1 on failure
 */
int DIGITALWrite(int io, int state);

/**
 * \brief Reads the analog channel on the IO board
 * \param the channel to read from [1..8]
 * \return a hex value representing the analog value
 */
int ANALOGRead(int channel);

/**
 * \brief Finds the Analog Voltage as read from the IO board
 * \return the voltage*100, on failure returns -1
 */
int ANALOGVoltage(void);

///@}

/******************************************************************************************************************************/

/** \name IRTV
 * Available functions for usage of the IR remote control
 */
///@{
/**
 * \brief	Blocking read of IRTV command and returns the keycode.
 * \return  Next code from the buffer
 */
int IRTVGet(void);

/**
 * \brief	Non-blocking read, return 0 if nothing and returns the keycode.
 * \return  Next code from the buffer
 */
int IRTVRead(void);

/**
 * \brief	Terminates the remote control decoder and releases the irtv thread.
 * \return  0 always
 */
int IRTVFlush(void);

/**
 * \brief		Checks to see if IR input is enabled;
 * \return		int retVal: whether IR input is enabled [on = 1, off =0]
 */
int IRTVGetStatus(void);

///@}

/******************************************************************************************************************************/

/** \name RADIO
 * Available functions for usage of the radio communication to the other eyebots
 */
///@{
/**
 * \brief		Starts radio communication between the Eyebots
 * \return		0 always
 */
int RADIOInit(void);

/**
 * \brief		Get own radio ID
 * \return		0 always
 */
int RADIOGetID(void);

/**
 * \brief		Send spec. number of bytes to ID destination
 * \param id_no: the id number of the eyebot
 * \param buf: the string to send
 * \return		0 always
 */
int RADIOSend(int id_no, char* buf);

/**
 * \brief		Read num. of bytes from ID source
 * \param id_no: the id number of the eyebot
 * \param buf: the bytes to receive
 * \param size: the size of the buffer to receive
 * \return		0 always
 */
int RADIOReceive(int *id_no, char* buf, int size);

/**
 * \brief		Non-blocking check whether message is waiting
 * \param       id_no: the id number of the eyebot to check
 * \return		0 always
 */
int RADIOCheck(int *id_no);

/**
 * \brief		Returns number of robots (incl. self) and list of IDs in network
 * \param       IDList[] the list of robots and ids
 * \return		0 always
 */
int RADIOStatus(int IDlist[]);

/**
 * \brief		Terminate radio communication
 * \return		0 always
 */
int RADIORelease(void);

/**
 * \brief   Returns the real position of the robot
 * \param x: integer to store the x coordinate of the robot (nearest mm)
 * \param y: integer to store the y coordinate of the robot (nearest mm)
 * \param phi: integer to store the angle the robot is facing (nearest degree)
 * \return  void
 */

void SIMGetPose(int *x, int *y, int *phi);

/**
 * \brief   Set the position of the robot in the simulator
 * \param x: new x coordinate for the robot (mm)
 * \param y: new y coordiante for the robot (mm)
 * \param phi: new angle of robots forward direction (degrees)
 */

void SIMSetPose(int x, int y, int phi);

/**
 * \brief   Get the position of object with ID in the simulation
 * \param x: integer to store the x coordinate of the object (mm)
 * \param y: integer to store the y coordinate of the object (mm)
 * \param phi: integer to store the angle of the object (degrees)
 */

void SIMGetObject(int id, int *x, int *y, int *phi);

/**
 * \brief   Set the position of object with ID in the simulation
 * \param x: new x coordinate of the object (mm)
 * \param y: new y coordinate of the object (mm)
 * \param phi: new angle of the object (degrees)
 */
void SIMSetObject(int id, int  x, int  y, int  phi);

/**
 * \brief   Perform a 360 degree distance sense
 * \param dists: a 360 element int array, to store the distances
 * \return  0 on success
 */
int SIMLaserScan(int * dists);

///@}

// Travis extra functions

//int initConnection();
int endConnection();
int receiveMessage(char*, unsigned char packetType);

#endif /* EYEBOT_H_ */
