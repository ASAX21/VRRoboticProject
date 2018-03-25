import eye, random
SAFE = 200

eye.LCDMenu("","","","END")
while (eye.KEYRead() != eye.KEY4):

    if (eye.PSDGet(1)>SAFE and eye.PSDGet(2)>SAFE and eye.PSDGet(3)>SAFE): 
      eye.LCDPrintf("straight\n", "")
      eye.VWStraight(10,500)
      eye.VWWait()

    else:
       eye.VWStraight(-25,50)
       eye.VWWait()
       eye.LCDPrintf("PSD %s \n", str(eye.PSDGet(2)))
       direc = int ((random.random() - 0.5) * 90)
       eye.VWTurn(direc, 90)
       eye.VWWait()
       eye.LCDPrintf("after turning \n", "")

