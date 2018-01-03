# Install scripts

## Windows

Uses Inno Setup Software (http://www.jrsoftware.org/isinfo.php). Modify the directories in the [Files] section to point to the build Unity application

## Mac

Uses Mac's packagebuilder to create a .pkg. Run build.sh to create the package. All files and folders in the "Files" directory will be built into the package. The structure should look like:

Files
    - Applications
        EyeSim.app
    
    -usr
        -local
            -bin
                gccsim
                g++sim
                eyesim
            -include
                eyebot.h
                types.h
            -lib
                libeyesim.a