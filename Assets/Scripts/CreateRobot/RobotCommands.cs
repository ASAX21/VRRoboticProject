using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// RobotCommand.cs contains the Command Pattern implemented by the simualtor.
/// ServerManager/Interpreter serves as the client, the Robot is the Invoker,
/// the Receiver is the Control structure, which acts on the component.
/// 
/// INTERPRETER --calls--> INVOKER --invokes--> COMMAND --commands--> RECEIVER --acts--> COMPONENT
/// 
/// 
/// ROBOT COMMANDS IS DEPRECATED THIS SUX no thank you
/// 
/// </summary>
/// <typeparam name="T"></typeparam>

// ----- COMMAND PATTERN INTERFACES -----

// Command interface
namespace RobotCommands
{
    public interface ICommand<T>
    {
        void Execute(T args);
    }

    // Can be Driven (motor torque)
    public interface IMotorControl
    {
        void SetMotorSpeed(int motor, float speed);
    }

    public interface IPIDControl
    {
        void SetMotorControlled(int motor, int ticks);
        void SetPIDParams(int motor, int p, int i, int d);
    }

	public interface ICameraControl
	{
		byte[] GetBytes (int camera);
        void SetResolution(int camera, int width, int height);
	}

    public interface IVWDriveControl
    {
        //void Initialize(int tick, int bs, int max, int dir);
        void SetSpeed(float linear, float angular);
        void DriveStraight(float speed, float distance);
        void DriveTurn(float rotSpeed, float angle);
        void DriveCurve(float speed, float distance, float angle);
        int DriveDone();
        //Speed GetSpeed();
        //int DriveRemaining();
    }

    // Can the Servo be set
    public interface IServoControl
    {
        void SetServoPosition(int servo, int position);
    }

    // Has PSD Sensors
    public interface IPSDSensorControl
    {
        UInt16 GetPSDValue(int psd);
    }

    // ----- CONCRETE COMMANDS -----

    // Drive Motor
    public class MotorDriveCommand : ICommand<int[]>
    {
        private readonly IMotorControl _drivable;

        public MotorDriveCommand(IMotorControl drivable)
        {
            _drivable = drivable;
        }

        public void Execute(int[] args)
        {
            // 0: Motor Index
            // 1: Speed
            _drivable.SetMotorSpeed(args[0], args[1]);
        }
    }

    public class MotorDriveControlledCommand : ICommand<int[]>
    {
        private readonly IPIDControl _drivable;

        public MotorDriveControlledCommand(IPIDControl drivable)
        {
            _drivable = drivable;
        }

        public void Execute(int[] args)
        {
            // 0: Motor index
            // 1: ticks
            _drivable.SetMotorControlled(args[0], args[1]);
        }
    }

    public class SetPIDCommand : ICommand<int[]>
    {
        private readonly IPIDControl _drivable;

        public SetPIDCommand(IPIDControl drivable)
        {
            _drivable = drivable;
        }

        public void Execute(int[] args)
        {
            // 0: Motor index
            // 1: P parameter
            // 2: I parameter
            // 3: D paramater
            _drivable.SetPIDParams(args[0], args[1], args[2], args[3]);
        }
    }

    public class SetServoCommand : ICommand<int[]>
    {
        private readonly IServoControl _servoSettable;

        public SetServoCommand(IServoControl servoSettable)
        {
            _servoSettable = servoSettable;
        }

        public void Execute(int[] args)
        {
            // 0: Servo Index
            // 1: Position
            _servoSettable.SetServoPosition(args[0], args[1]);
        }
    }

    public class GetPSDSensorValueCommand : ICommand<int>
    {
        private readonly IPSDSensorControl _psdSensors;
        public UInt16 _value { get; private set; }

        public GetPSDSensorValueCommand(IPSDSensorControl psdSensors)
        {
            _psdSensors = psdSensors;
        }

        public void Execute(int args)
        {
            // args: PSD Index
            _value = _psdSensors.GetPSDValue(args);
        }
    }

    /*
    public class InitalizeVWControlCommand : ICommand<int[]>
    {
        private readonly IVWDriveControl _vwControllable;

        public InitalizeVWControlCommand(IVWDriveControl vwControllable)
        {
            _vwControllable = vwControllable;
        }

        public void Execute(int[] args)
        {
            // 0: ticks
            // 1: base
            // 2: max
            // 2: dir
            _vwControllable.Initialize(args[0], args[1], args[2], args[3]);
        }
    }

    public class VWSetSpeedCommand : ICommand<int[]>
    {
        private readonly IVWDriveControl _vwDrivable;

        public VWSetSpeedCommand(IVWDriveControl vwControllable)
        {
            _vwDrivable = vwControllable;
        }

        public void Execute(int[] args)
        {
            // 0: linear
            // 1: angular
            _vwDrivable.SetSpeed(args[0], args[1]);
        }
    }

    public class VWGetSpeedCommand : ICommand<int[]>
    {
        private readonly IVWDriveControl _vwDrivable;
        public Speed speed { get; private set; }

        public VWGetSpeedCommand(IVWDriveControl vwControllable)
        {
            _vwDrivable = vwControllable;
        }

        public void Execute(int[] args)
        {
            // 0: linear
            // 1: angular
            speed = _vwDrivable.GetSpeed();
        }
    }

    public class VWDriveStraightCommand : ICommand<int[]>
    {
        private readonly IVWDriveControl _vwDrivable;

        public VWDriveStraightCommand(IVWDriveControl vwControllable)
        {
            _vwDrivable = vwControllable;
        }

        public void Execute(int[] args)
        {
            // 0: speed
            // 1: distance
            _vwDrivable.DriveStraight(args[0], args[1]);
        }
    }

    public class VWDriveTurnCommand : ICommand<int[]>
    {
        private readonly IVWDriveControl _vwDrivable;

        public VWDriveTurnCommand(IVWDriveControl vwControllable)
        {
            _vwDrivable = vwControllable;
        }

        public void Execute(int[] args)
        {
            // 0: rotSpeed
            // 1: angle
            _vwDrivable.DriveTurn(args[0], args[1]);
        }
    }

    public class VWDriveCurveCommand : ICommand<int[]>
    {
        private readonly IVWDriveControl _vwDrivable;

        public VWDriveCurveCommand(IVWDriveControl vwControllable)
        {
            _vwDrivable = vwControllable;
        }

        public void Execute(int[] args)
        {
            // 0: speed
            // 1: distance
            // 2: angle
            _vwDrivable.DriveCurve(args[0], args[1], args[2]);
        }
    }
   
    public class VWDriveRemainingCommand : ICommand<int[]>
    {
        private readonly IVWDriveControl _vwDrivable;
        public int remaining { get; private set; }

        public VWDriveRemainingCommand(IVWDriveControl vwControllable)
        {
            _vwDrivable = vwControllable;
        }

        public void Execute(int[] args)
        {
            remaining = _vwDrivable.DriveRemaining();
        }
    }

    public class VWDriveDoneCommand : ICommand<int[]>
    {
        private readonly IVWDriveControl _vwDrivable;
        public int done { get; private set; }

        public VWDriveDoneCommand(IVWDriveControl vwControllable)
        {
            _vwDrivable = vwControllable;
        }

        public void Execute(int[] args)
        {
            done = _vwDrivable.DriveDone();
        }
    }
    */
    public class GetCameraOutputCommand : ICommand<int>
    {
		private readonly ICameraControl _cameraControl;
        public byte[] img { get; private set; }

        public GetCameraOutputCommand(ICameraControl cameraControl)
        {
            _cameraControl = cameraControl;
        }

        public void Execute(int args)
        {
           img = _cameraControl.GetBytes(args);
        }
    }

    public class SetCameraResolutionCommand : ICommand<int[]>
    {
        private readonly ICameraControl _cameraControl;
        
        public SetCameraResolutionCommand(ICameraControl cameraControl)
        {
            _cameraControl = cameraControl;
        }

        public void Execute(int[] args)
        {
            // 0 : camera
            // 1 : width
            // 2 : height
            _cameraControl.SetResolution(args[0], args[1], args[2]);
        }
           
    }
}