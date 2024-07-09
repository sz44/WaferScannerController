using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QMProjectTektronix
{
    public class UserInputHandler
    {
        private StageController sc;
        private AlignerController ac;

        private  StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;

        public UserInputHandler(StageController stageControls, AlignerController alignerControls)
        {
            sc = stageControls;
            ac = alignerControls;     
        }
  
        public async Task<string> UserCommand(string command)
        {
            string res = "";
            var splitInput = command.Split();
            var code = splitInput[0];

            if (stringComparer.Equals("quit", code))
            {
                Quit();
            }
            else if (stringComparer.Equals("home", code))
            {
                Home(splitInput);
            }
            else if (stringComparer.Equals("moveAbs", code))
            {
                MoveAbsolute(splitInput);
            }
            else if (stringComparer.Equals("moveRel", code))
            {
                MoveRelative(splitInput);
            }
            else if (stringComparer.Equals("setAbs", code))
            {
                SetAbsolute(splitInput);
            }
            else if (stringComparer.Equals("setRel", code))
            {
                SetRelative(splitInput);
            }
            else if (stringComparer.Equals("pos", code))
            {
                Position(splitInput);
            }
            else if (stringComparer.Equals("dis", code))
            {
                Distance(splitInput);
            }
            else if (stringComparer.Equals("vel", code))
            {
                Velocity(splitInput);
            }
            else if (stringComparer.Equals("accel", code))
            {
                Acceleration(splitInput);
            }
            else if (stringComparer.Equals("decel", code))
            {
                Deceleration(splitInput);
            }
            else if (stringComparer.Equals("reset", code))
            {
                Reset();
            }
            else if (stringComparer.Equals("joyfast", code))
            {
                JoyStickFast(splitInput);
            }
            else if (stringComparer.Equals("joyslow", code))
            {
                JoyStickSlow(splitInput);
            }
            else if (stringComparer.Equals("joyoff", code))
            {
                JoyStickOff(splitInput);
            }
            else if (stringComparer.Equals("routine1", code))
            {
                Routines.Routine1(sc, ac);
            }
            else if (stringComparer.Equals("alignwafer300", code))
            {
                await Routines.AlignWafer(sc, ac, 300);
            }
            else if (stringComparer.Equals("alignwafer200", code))
            {
                await Routines.AlignWafer(sc, ac, 200);
            }
            else if (stringComparer.Equals("alignwafer150", code))
            {
                await Routines.AlignWafer(sc, ac, 150);
            }
            else if (stringComparer.Equals("pickupwafer", code))
            {
                await Routines.WaferPickUpPosition(sc, ac);
            }
            else if (stringComparer.Equals("pickupandalign", code))
            {
                await Routines.PickUpWaferAndAlignCycle(sc, ac, 300);
            }
            else if (stringComparer.Equals("error", code))
            {
                Error(splitInput);
            }
            else if (stringComparer.Equals("on", code))
            {
                MotorOn(splitInput);
            }
            else if (stringComparer.Equals("off", code))
            {
                MotorOff(splitInput);
            }
            else if (stringComparer.Equals("fsol", code))
            {
                Fsol(splitInput);
            }
            else if (stringComparer.Equals("read", code))
            {
                sc._conn.ReadBytes();
                Console.WriteLine(res);
            }
            else if (stringComparer.Equals("aread", code))
            {
                res = ac._conn.ReadBytes();
                Console.WriteLine(res);
            }
            else if (stringComparer.Equals("aclear", code))
            {
                AlignerClear();
            }
            else if (stringComparer.Equals("a", code))
            {
                AlignerCommand(splitInput);
            }
            else if (stringComparer.Equals("joyonall", code))
            {
                sc.JoyStickFast();
            }
            else if (stringComparer.Equals("joyoffall", code))
            {
                sc.JoyStickOff();
            }
            else if (stringComparer.Equals("stop", code))
            {
                Stop();
            }
            else if (stringComparer.Equals("center", code))
            {
                Center(splitInput);
            }
            else if (stringComparer.Equals("poslimit", code))
            {
                PositiveLimit(splitInput);
            }
            else if (stringComparer.Equals("vacuumOn", code))
            {
                ac.VacuumOn();
            }
            else if (stringComparer.Equals("vacuumOff", code))
            {
                ac.VacuumOff();
            }
            else if (stringComparer.Equals("vacuumstatus", code))
            {
                VacuumStatus();
            }
            else if (stringComparer.Equals("gripstatus", code))
            {
                GripStatus();
            }
            else if (stringComparer.Equals("tbreakstatus", code))
            {
                TBreakStatus();
            }
            else if (stringComparer.Equals("waferstatus", code))
            {
                WaferStatus();
            }
            else if (stringComparer.Equals("grip", code))
            {
                sc.Fsol(1, "on");
            }
            else if (stringComparer.Equals("ungrip", code))
            {
                sc.Fsol(2, "on");
            }
            else if (stringComparer.Equals("tbreak", code))
            {
                TBreak();
            }
            else if (stringComparer.Equals("rotatewafer", code))
            {
                RotateWafer(splitInput);
            }
            else if (stringComparer.Equals("joystatus", code))
            {
                JoystickStatus();
            }
            else
            {
                sc.Send(command);
            }
            return res;
        }
        public void JoystickStatus()
        {
            Console.WriteLine($" x: {((sc.JoyStickDict["x"])==true ? "on" : "off")}");
            Console.WriteLine($" y: {((sc.JoyStickDict["y"]) == true ? "on" : "off")}");
            Console.WriteLine($" z: {((sc.JoyStickDict["z"]) == true ? "on" : "off")}");
            Console.WriteLine($" t: {((sc.JoyStickDict["t"]) == true ? "on" : "off")}");
        }
        public async Task TBreak()
        {
            if(await sc.TBreakOn())
            {
                sc.Fsol(5, "on");
                Console.WriteLine("TBreak turned off");
            }
            else
            {
                sc.Fsol(5, "off");
                Console.WriteLine("TBreak turned on");
            }
        }

        public async Task TBreakStatus()
        {
            bool tbreak = await sc.TBreakOn();
            //bool close = await sc.GripperClosed();
            if (tbreak)
            {
                Console.WriteLine("break on");
            }
            else
            {
                Console.WriteLine("break off");
            }
        }

        public async Task GripStatus()
        {
            bool open = await sc.GripperOpen();           
            if (open)
            {
                Console.WriteLine("gripper opened");
            }
            else
            {
                Console.WriteLine("gripper closed");
            }
        }
        public async Task Center(string[] input)
        {
            string axis;
            if (input.Length < 2)
            {
                Console.WriteLine("no axis given");
            }
            else
            {               
                axis = input[1];
                try
                {
                    await sc.MoveAbsoluteAsync(axis, Positions.Center[axis]);
                }
                catch (OperationFailedException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }            
        }

        public async Task PositiveLimit(string[] input)
        {
            string axis;
            if (input.Length < 2)
            {
                Console.WriteLine("no axis given");
            }
            else
            {
                axis = input[1];
                try
                {
                    await sc.MoveAbsoluteAsync(axis, Positions.PosLimit[axis]);
                }
                catch (OperationFailedException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        
        public void Quit()
        {                
            Program.End = true;
        }
        public void Stop()
        {
            Routines.Stop = true;
            sc.Stop();
        }

        public void Print(int value)
        {
            Console.WriteLine($"response: {value}");
        }    
        public void Reset()
        {
            sc.Reset();
            Console.WriteLine("completed reset");
        }

        public async Task RotateWafer(string[] input)
        {
            if (input.Length < 2)
            {
                Console.WriteLine("no value given");
            }
            else 
            {
                var value = input[1];
                await ac.RotateWafer(value);
            }
        }

        public async Task Error(string[] input)
        {
            if (input.Length < 2)
            {
                Console.WriteLine("no axis given");
            }
            else
            {
                var axis = input[1];

                int errorNum = await sc.Error(axis);

                Console.WriteLine($"error code: {errorNum}");

                foreach (int key in sc.Errors.Keys)
                {
                    if ((errorNum & key) == key)
                    {
                        Console.WriteLine($"{Math.Log10(key) / Math.Log10(2)} : {sc.Errors[key]}");
                    }
                }
            }
        }

        public void JoyStickFast(string[] input)
        {
            if(input.Length < 2)
            {
                Console.WriteLine("no axis given");
            }
            else
            {
                var axis = input[1];
                sc.JoyStickFast(axis);
            }
        }

        
        public void JoyStickSlow(string[] input)
        {
            if (input.Length < 2)
            {
                Console.WriteLine("no axis given");
            }
            else
            {
                var axis = input[1];
                sc.JoyStickSlow(axis);
            }
        }
        public void JoyStickOff(string[] input)
        {
            try
            {
                var axis = input[1];
                sc.JoyStickOff(axis);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }

        public void Deceleration(string[] input)
        {
            string axis;
            if (input.Length == 1)
            {
                Console.WriteLine("no axis given");
            }
            else if (input.Length == 2) //get
            {
                axis = input[1];
                sc.Deceleration(axis, null);
            }
            else if (input.Length == 3) //set
            {
                axis = input[1];
                int.TryParse(input[2], out int value);
                sc.Deceleration(axis, value);
            }
        }
        public void Acceleration(string[] input)
        {
            string axis;
            if (input.Length == 1)
            {
                Console.WriteLine("no axis given");
            }
            else if (input.Length == 2)
            {
                axis = input[1];
                sc.Acceleration(axis, null);
            }
            else if (input.Length == 3)
            {
                axis = input[1];
                int.TryParse(input[2], out int value);              
                sc.Acceleration(axis, value);
            }
        }
        public void Velocity(string[] input)
        {
            string axis;
            if (input.Length==1)
            {
                Console.WriteLine("no axis given");             
            }
            else if(input.Length==2)
            {
                axis = input[1];
                sc.Velocity(axis, null);
                
            }
            else if (input.Length == 3)
            {
                axis = input[1];
                int.TryParse(input[2], out int value);
                sc.Velocity(axis, value);
            }
        }
        
        public async Task Position(string[] input)
        {
            if (input.Length<2)
            {
                Console.WriteLine("no axis given");
            } 
            else
            {
                var axis = input[1];
                int pos = await sc.Position(axis);
                Console.WriteLine($"position: {pos}");
            }       
        }
        public void Distance(string[] input)
        {
            string axis;           
            
            if (input.Length == 1)
            {
                Console.WriteLine("no axis given");              
            }
            else if (input.Length == 2)
            {
                axis = input[1];
                sc.Distance(axis, null);
            } 
            else if (input.Length == 3)
            {
                axis = input[1];
                int.TryParse(input[2], out int value);
                sc.Distance(axis, value);
            }
        }
        public void SetAbsolute(string[] input)
        {
            if (input.Length<2)
            {
                Console.WriteLine("no axis given");
            }
            else
            {
                var axis = input[1];
                sc.SetAbsolute(axis);
            }
        }
        public void SetRelative(string[] input)
        {
            if (input.Length < 2)
            {
                Console.WriteLine("no axis given");
            }
            else
            {
                var axis = input[1];
                sc.SetRelative(axis);
            }
        }
        
        public async Task Home(string[] input)
        {           
            string axis;
            if (input.Length<2)
            {
                Console.WriteLine("no axis given");               
            }
            else
            {
                axis = input[1];             
                try
                {
                    await sc.HomeStage(axis);
                    Console.WriteLine($"homeing {axis} axis");
                }
                catch (OperationFailedException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }           
        }      

        public async Task MoveAbsolute(string[] input)
        {
            string axis;
            int position;
            if (input.Length<3)
            {              
                Console.WriteLine("no axis or no position given");
            }
            else
            {
                axis = input[1];              
                int.TryParse(input[2], out position);
                try
                {
                    await sc.MoveAbsoluteAsync(axis, position);
                    Console.WriteLine("moving started");
                }
                catch(OperationFailedException ex)
                {
                    Console.WriteLine(ex.Message);
                }  
            }
        }

        
        public async Task MoveRelative(string[] input)
        {
            string axis;
            int position;
            if (input.Length < 3)
            {
                Console.WriteLine("no axis or no position given");
            }
            else
            {
                axis = input[1];
                int.TryParse(input[2], out position);
                try
                {
                    await sc.MoveRelativeAsync(axis, position);
                    Console.WriteLine("moving started");
                }
                catch (OperationFailedException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }          
        }

        public void MotorOn(string[] input)
        {
            if (input.Length < 2)
            {
                Console.WriteLine("no axis given");
            }
            else
            {
                var axis = input[1];
                sc.MotorOn(axis);
            }
        }
        public void MotorOff(string[] input)
        {
            if (input.Length < 2)
            {
                Console.WriteLine("no axis given");
            }
            else
            {
                var axis = input[1];
                sc.MotorOff(axis);
            }
        }

        public void Fsol(string[] input)
        {
            //format: fsol <number> <command>
            try
            {
                int number = Int32.Parse(input[1]);
                try
                {
                    var command = input[2];
                    sc.Fsol(number, command);
                }
                catch (IndexOutOfRangeException)
                {
                    sc.Fsol(number);
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no number given");
            }
        }

        public async Task VacuumStatus()
        {
            await ac.VacuumStatus();
        }

        public async Task WaferStatus()
        {
            bool status = await sc.WaferSensor();
            //bool close = await sc.GripperClosed();
            if (status)
            {
                Console.WriteLine("wafer is sensed");
            }
            else
            {
                Console.WriteLine("no wafer");
            }
        }

        public void AlignerClear()
        {
            ac.Clear();
        }

        public void AlignerCommand(string[] input)
        {
            //string res = ac.Command(String.Join(" ",input.Skip(1)));
            ac.Command(String.Join(" ", input.Skip(1)));
            //Console.WriteLine(res);
        }
        public void AlignerConfig()
        {
            ac.Config();          
        }

        public int Axis(string a)
        {
            switch (a)
            {
                case "x":
                    return 0;
                case "y":
                    return 1;
                case "z":
                    return 2;
                case "t":
                    return 3;
                default:
                    return -1;
            }
        }
    }
}
