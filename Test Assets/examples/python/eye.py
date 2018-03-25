import ctypes
eye = ctypes.CDLL('cygeyesim.dll')
eye.KEYGet.argtypes = (None)
eye.KEYGetXY.argtypes = (ctypes.POINTER(ctypes.c_int),ctypes.POINTER(ctypes.c_int))


eye.VWDrive.argtypes = (ctypes.c_int, ctypes.c_int,ctypes.c_int)
eye.VWCurve.argtypes = (ctypes.c_int, ctypes.c_int)
eye.VWTurn.argtypes = (ctypes.c_int, ctypes.c_int)
eye.VWStraight.argtypes = (ctypes.c_int, ctypes.c_int)
eye.VWSetSpeed.argtypes = (ctypes.c_int, ctypes.c_int)
eye.VWSetPosition.argtypes = (ctypes.c_int, ctypes.c_int,ctypes.c_int)
eye.VWDone.argtypes = (None)
eye.VWRemain.argtypes = (None)
eye.VWWait.argtypes = (None)
eye.VWStalled.argtypes = (None)

eye.LCDClear.argtypes = (None)
eye.LCDRefresh.argtypes = (None)
eye.LCDSetPos.argtypes = (ctypes.c_int, ctypes.c_int)
eye.LCDSetFont.argtypes = (ctypes.c_int, ctypes.c_int)
eye.LCDPixelInvert.argtypes = (ctypes.c_int, ctypes.c_int)
eye.LCDLineInvert.argtypes = (ctypes.c_int, ctypes.c_int,ctypes.c_int, ctypes.c_int)
eye.LCDAreaInvert.argtypes = (ctypes.c_int, ctypes.c_int,ctypes.c_int, ctypes.c_int)
eye.LCDCircleInvert.argtypes = (ctypes.c_int,ctypes.c_int,ctypes.c_int)
eye.LCDImageStart.argtypes = (ctypes.c_int,ctypes.c_int,ctypes.c_int,ctypes.c_int)

eye.LCDMenu.argtypes = (ctypes.c_char_p,ctypes.c_char_p,ctypes.c_char_p,ctypes.c_char_p)
eye.LCDSetPrintf.argtypes = (ctypes.c_int,ctypes.c_int,ctypes.c_char_p)
eye.PSDGet.restypes = ctypes.c_int
eye.KEYGet.restypes = ctypes.c_int
eye.LCDPrintf.argtypes = (ctypes.c_char_p,ctypes.c_char_p)
eye.LCDGetPos.argtypes = (ctypes.POINTER(ctypes.c_int),ctypes.POINTER(ctypes.c_int))
eye.LCDGetPos.restypes = ctypes.c_int
eye.LCDSetColor.argtypes = (ctypes.c_int, ctypes.c_int)
eye.LCDGetSize.argtypes = (ctypes.POINTER(ctypes.c_int),ctypes.POINTER(ctypes.c_int))
eye.LCDGetSize.restypes = ctypes.c_int
eye.LCDPixel.argtypes = (ctypes.c_int,ctypes.c_int,ctypes.c_int)
eye.LCDGetPixel.argtypes = (ctypes.c_int, ctypes.c_int)
eye.LCDGetPixel.restypes = ctypes.c_int
eye.LCDLine.argtypes = (ctypes.c_int, ctypes.c_int,ctypes.c_int, ctypes.c_int,ctypes.c_int)
eye.LCDArea.argtypes = (ctypes.c_int, ctypes.c_int,ctypes.c_int, ctypes.c_int,ctypes.c_int,ctypes.c_int)
eye.LCDCircle.argtypes = (ctypes.c_int,ctypes.c_int,ctypes.c_int,ctypes.c_int,ctypes.c_int)



eye.CAMRelease.argtypes = (None)

eye.OSMachineSpeed.argtypes =(None)
eye.OSMachineType.argtypes =(None)
eye.OSMachineID.argtypes = (None)
eye.OSGetCount.argtypes = (None)

eye.AUBeep.argtypes = (None)
eye.AUDone.argtypes = (None)
eye.AUMicrophone.argtypes = (None)

eye.SERVOSet.argtypes = (ctypes.c_int,ctypes.c_int)
eye.SERVOSetRaw.argtypes = (ctypes.c_int,ctypes.c_int)
eye.SERVORange.argtypes = (ctypes.c_int,ctypes.c_int)
eye.MOTORDrive.argtypes = (ctypes.c_int,ctypes.c_int)
eye.MOTORDriveRaw.argtypes = (ctypes.c_int,ctypes.c_int)
eye.MOTORPID.argtypes = (ctypes.c_int,ctypes.c_int,ctypes.c_int,ctypes.c_int)
eye.MOTORSpeed.argtypes = (ctypes.c_int,ctypes.c_int)


def KEYRead():
    global eye
    result = eye.KEYRead()
    return int(result)

def PSDGet(psd):
    global eye
    result = eye.PSDGet(ctypes.c_int(psd))
    return int(result)

def PSDGetRaw(psd):
    global eye
    result = eye.PSDGetRaw(ctypes.c_int(psd))
    return int(result)

def CAMInit(res):
    global eye
    eye.CAMInit(ctypes.c_int(res))

def OSWait(n):
    global eye
    eye.OSWait(ctypes.c_int(n))

def CAMGet(buf):
    global eye
    pi = POINTER(ctypes.c_char)
    result = eye.CAMGet(pi)
    return BYTE * (result)

def LCDMenu(st1,st2,st3,st4):
    global eye
    a = ctypes.c_char_p(st1)
    b = ctypes.c_char_p(st2)
    c = ctypes.c_char_p(st3)
    d = ctypes.c_char_p(st4)
    eye.LCDMenu(a,b,c,d)

def LCDSetPrintf(row,column,fmt):
    global eye
    r = ctypes.c_int(row)
    c = ctypes.c_int(column)
    f = ctypes.c_char_p(fmt)
    eye.LCDSetPrintf(r,c,f)

def LCDPrintf(a,b):
    global eye
    eye.LCDPrintf(ctypes.c_char_p(a),ctypes.c_char_p(b))


def LCDGetPos(a,b):
    global eye
    ap = POINTER(ctypes.c_int)
    bp = POINTER(ctypes.c_int)
    result = eye.LCDGetPos(ap,bp)
    return int(result)


def LCDSetColor(fg,bg):
    global eye
    eye.LCDSetColor(ctypes.c_int(fg),ctypes.c_int(bg))

def LCDGetSize(x,y):
    global eye
    xp = POINTER(ctypes.c_int)
    yp = POINTER(ctypes.c_int)
    result = eye.LCDGetSize(xp,yp)
    return int(result)

def LCDPixel(x,y,col):
    global eye
    eye.LCDPixel(ctypes.c_int(x),ctypes.c_int(y),ctypes.c_int(col))

def LCDGetPixel(x,y):
    global eye
    eye.LCDGetPixel(ctypes.c_int(x),ctypes.c_int(y))


def LCDLine(x,y,xx,yy,col):
    global eye
    eye.LCDLine(ctypes.c_int(x),ctypes.c_int(y),ctypes.c_int(xx),ctypes.c_int(yy),ctypes.c_int(col))


def LCDArea(x,y,xx,yy,col,fill):
    global eye
    eye.LCDArea(ctypes.c_int(x),ctypes.c_int(y),ctypes.c_int(xx),ctypes.c_int(yy),ctypes.c_int(col),ctypes.c_int(fill))


def LCDCircle(x,y,size,col,fil):
    global eye
    eye.LCDCircle(ctypes.c_int(x),ctypes.c_int(y),ctypes.c_int(size),ctypes.c_int(col),ctypes.c_int(fil))









def KEYGet():
    global eye
    eye.KEYGet(None)
    return int(result)

def KEYRead():
    global eye
    result = eye.KEYRead(None)
    return int(result)

def KEYWait(key):
    global eye
    eye.KEYWait(ctypes.c_int(key))

    
def VWDrive(dis,vel,spd):
    global eye
    eye.VWDrive(ctypes.c_int(dis),ctypes.c_int(vel),ctypes.c_int(spd))

def VWCurve(dist,angle,speed):
    global eye
    eye.VWCurve(ctypes.c_int(dist),ctypes.c_int(angle),ctypes.c_int(speed))

def VWTurn(ang, speed):
    global eye
    eye.VWTurn(ctypes.c_int(ang), ctypes.c_int(speed))

def VWStraight(dist, speed):
    global eye
    eye.VWStraight(ctypes.c_int(dist), ctypes.c_int(speed))


def VWSetSpeed(speed,angspeed):
    global eye
    eye.VWSetSpeed(ctypes.c_int(speed), ctypes.c_int(angspeed))

def VWSetPosition(x,y,phi):
    global eye
    eye.VWSetPosition(ctypes.c_int(x), ctypes.c_int(y),ctypes.c_int(phi))

def VWDone():
    global eye
    result = eye.VWDone(None)
    return int(result)

    
def VWRemain():
    global eye
    result = eye.VWRemain(None)
    return int(result)

def VWWait():
    global eye
    eye.VWWait(None)


def VWStalled():
    global eye
    result = eye.VWStalled(None)
    return int(result)


def LCDSetMode(mode):
    global eye
    eye.LCDSetMode(ctypes.c_int(mode))

def LCDClear():
    global eye
    eye.LCDClear(None)

def LCDRefresh():
    global eye
    eye.LCDRefresh(None)

def LCDSetPos(row, column):
    global eye
    eye.LCDSetPos(ctypes.c_int(row),ctypes.c_int(column))

def LCDSetFont(font, variation):
    global eye
    eye.LCDSetFont(ctypes.c_int(font),ctypes.c_int(variation))

def LCDFontSize(size):
    global eye
    eye.LCDFontSize(ctypes.c_int(size))

def LCDPixelInvert(x,y):
    global eye
    eye.LCDPixelInvert(ctypes.c_int(x),ctypes.c_int(y))

def LCDLineInvert(x,y,xx,yy):
    global eye
    eye.LCDLineInvert(ctypes.c_int(x),ctypes.c_int(y),ctypes.c_int(xx),ctypes.c_int(yy))

def LCDAreaInvert(x,y,xx,yy):
    global eye
    eye.LCDAreaInvert(ctypes.c_int(x),ctypes.c_int(y),ctypes.c_int(xx),ctypes.c_int(yy))

def LCDCircleInvert(x,y,size):
    global eye
    eye.LCDCircleInvert(ctypes.c_int(x),ctypes.c_int(y),ctypes.c_int(size))

def LCDImageSize(x):
    global eye
    eye.LCDImageSize(ctypes.c_int(x))

def LCDImageStart(x,y,xs,ys):
    global eye
    eye.LCDImageSize(ctypes.c_int(x),ctypes.c_int(y),ctypes.c_int(xs),ctypes.c_int(ys))

def CAMInit(res):
    global eye
    eye.CAMInit(ctypes.c_int(res))

def CAMRelease():
    global eye
    eye.CAMRelease(None)

def OSVersion(buf):
    global eye
    eye.OSVersion(ctypes.c_char_p(buf))

def OSVersionIO(buf):
    global eye
    eye.OSVersionIO(ctypes.c_char_p(buf))

def OSMachineSpeed():
    global eye
    eye.OSMachineSpeed(None)

def OSMachineType():
    global eye
    eye.OSMachineType(None)

def OSMachineName(buf):
    global eye
    eye.OSMachineName(ctypes.c_char_p(buf))

def OSMachineID():
    global eye
    eye.OSMachineID(None)

def OSGetCount():
    global eye
    eye.OSGetCount(None)

def AUBeep():
   global eye
   eye.AUBeep(None)

def AUDone():
    global eye
    eye.AUDone(None)

def AUPlay(filename):
    global eye
    eye.AUPlay(ctypes.c_char_p(filename))

def AUMicrophone():
    global eye
    eye.AUMicrophone(None)

def SERVOSet(servo, angle):
    global eye
    eye.SERVOSet(ctypes.c_int(servo), ctypes.c_int(angle))

def SERVOSetRaw(servo, angle):
    global eye
    eye.SERVOSetRaw(ctypes.c_int(servo), ctypes.c_int(angle))
    
def SERVORange(servo, low,high):
    global eye
    eye.SERVORange(ctypes.c_int(servo), ctypes.c_int(low),ctypes.c_int(high))

def MOTORDrive(motor, speed):
    global eye
    eye.MOTORDrive(ctypes.c_int(motor), ctypes.c_int(speed))

def MOTORDriveRaw(motor, speed):
    global eye
    eye.MOTORDriveRaw(ctypes.c_int(motor), ctypes.c_int(speed))
    
def MOTORPID(motor, p,i,d):
    global eye
    eye.MOTORPID(ctypes.c_int(motor), ctypes.c_int(p),ctypes.c_int(i),ctypes.c_int(d))

def MOTORPIDOff(motor):
    global eye
    eye.MOTORPIDOff(ctypes.c_int(motor))

def MOTORSpeed(motor, speed):
    global eye
    eye.MOTORSpeed(ctypes.c_int(motor), ctypes.c_int(speed))
       
def ENCODERRead(quad):
    global eye
    eye.ENCODERRead(ctypes.c_int(quad))

def ENCODERReset(quad):
    global eye
    eye.ENCODERReset(ctypes.c_int(quad))



def IPSetSize(res):
    global eye
    eye.IPSetSize(ctypes.c_int(res))


