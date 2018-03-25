import ctypes
eye = ctypes.CDLL('../../lib/dynamic/libeyesim.dylib')
eye.KEYGet.argtypes = (None)
eye.KEYGetXY.argtypes = (ctypes.POINTER(ctypes.c_int),ctypes.POINTER(ctypes.c_int))

KEY1 = 1
KEY2 = 2
KEY3 = 4
KEY4 = 8

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

def KEYRead():
    global eye
    result = eye.KEYRead()
    return int(result)

def PSDGet(psd):
    global eye
    result = eye.PSDGet(ctypes.c_int(psd))
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

def KEYGetXY(x,y):
    xp = POINTER(ctypes.c_int)
    yp = POINTER(ctypes.c_int)
    result = eye.KEYGetXY(xp,yp)
    return int(result)

    
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

