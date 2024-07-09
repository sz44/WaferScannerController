using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace QMProjectTektronix
{
    public class AlignerController
    {
        public Connection _conn;
        public AlignerController(Connection comm)
        {
            Console.WriteLine("initializing aligner controls...");
            _conn = comm;
        }

        public void End()
        {         
            //close port
            _conn.End();
        }

        public async Task Escape()
        {
            string ascii = "ESC";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;          
        }

        /// <summary>
        /// Lowers chuck
        /// </summary>
        /// <returns></returns>
        public async Task<string> MoveDown()
        {
            string ascii = "ZMD";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
            return res;
        }

        public async Task<bool> VacuumStatus()
        {
            string ascii = "RVC";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;           
            if (res=="\n255\r")
            {
                Console.WriteLine("vacuum status: on");
                return true;
            }
            Console.WriteLine("vacuum status: off");
            return false;
        }

        /// <summary>
        /// Loops until chuck has completed move to down position
        /// </summary>
        /// <returns></returns>
        public async Task WaitForDown()
        {
            Console.WriteLine("waiting for chuck down");

            CancellationTokenSource source = new CancellationTokenSource(10000);

            while (!source.IsCancellationRequested)
            {  
                string ascii = "ZRS";
                Command command = new Command(ascii);
                _conn.AddCommand(command);
                string res = await command.TSC.Task;
                if (res == "\nD\r")                
                {
                    Console.WriteLine("Chuck reached up postion");
                    return;
                }
                if (res == null)
                {
                    throw new OperationFailedException("could not read z position status");
                }
            }
            throw new OperationFailedException("Moving up timedout");
        }

        /// <summary>
        /// Loops until chuck has completed move to up position
        /// </summary>
        /// <returns></returns>
        public async Task WaitForUp()
        {
            Console.WriteLine("waiting for chuck up");

            CancellationTokenSource source = new CancellationTokenSource(10000);

            while (!source.IsCancellationRequested)
            {             
                string ascii = "ZRS";
                Command command = new Command(ascii);
                _conn.AddCommand(command);
                string res = await command.TSC.Task;
                if (res == null)
                {
                    throw new OperationFailedException("could not read z position status");
                }
                if (res == "\nU\r")
                {
                    Console.WriteLine("Chuck reached up postion");
                    return;
                }              
            }
            throw new OperationFailedException("Moving up timedout");
        }

        /// <summary>
        /// Loops until align ends
        /// </summary>
        /// <returns></returns>
        public async Task WaitForAlign()
        {
            Console.WriteLine("waiting for align");

            CancellationTokenSource source = new CancellationTokenSource(10000);

            while (!source.IsCancellationRequested)
            {
                //create command
                string ascii = "ASG";
                Command command = new Command(ascii);
                _conn.AddCommand(command);
                //get response
                string res = await command.TSC.Task;

                if (res==null)
                {
                    throw new OperationFailedException("could not read align status");
                }                           
                if (res == "\nC\r")
                {
                    Console.WriteLine("align complete");
                    return;
                }
                if (res == "\nF\r")
                {
                    throw new OperationFailedException("align failed");                   
                }
            }
            throw new OperationFailedException("align timed out");
        }

        /// <summary>
        /// Raises chuck up
        /// </summary>
        public void MoveUp()
        {
            //note: using ZMX instead of ZMU because ZRS (for getting z-axis status) does not work correctly with ZMU.
            string ascii = "ZMX";
            Command command = new Command(ascii);
            _conn.AddCommand(command);           
        }

        /// <summary>
        /// Turns vacuum on
        /// </summary>
        /// <returns></returns>
        public async Task VacuumOn()
        {
            string ascii = "VAC1";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
        }
        /// <summary>
        /// Turns off vacuum
        /// </summary>
        public void VacuumOff()
        {
            string ascii = "VAC0";
            Command command = new Command(ascii);
            _conn.AddCommand(command);          
        }

        /// <summary>
        /// Loops untill vacuum has been turned on
        /// </summary>
        /// <returns></returns>
        public async Task WaitVacuumOn()
        {
            Console.WriteLine("waiting for vacuum on");

            CancellationTokenSource source = new CancellationTokenSource(10000);

            while (!source.IsCancellationRequested)
            {                  
                bool status = await VacuumStatus();

                if (status)
                {
                    return;
                }                    
            }
            throw new OperationFailedException("failed: vacuum did not turn on");
        }

        /// <summary>
        /// Loops until vacuum has been shut off
        /// </summary>
        /// <returns></returns>
        public async Task WaitVacuumOff()
        {
            Console.WriteLine("waiting for vacuum off");

            CancellationTokenSource source = new CancellationTokenSource(5000);

            while (!source.IsCancellationRequested)
            {             
                bool status = await VacuumStatus();

                if (!status)
                {
                    return;
                }
            }

            throw new OperationFailedException("failed: vacuum did not turn off");
        }

        /// <summary>
        /// Raises chuck up while keeping vacuum on
        /// </summary>
        /// <returns></returns>
        public async Task ZVacuumUp()
        {
            string ascii = "ZVMX";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
        }

        /// <summary>
        /// Find notch or flat of wafer
        /// </summary>
        /// <returns></returns>
        public async Task Align()
        {
            await Escape();
            string ascii = "APF";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
        }

        public async Task RotateWafer(string n)
        {
            await Escape();
            
            string head = "MAM ";
            string tail = "000";
            string ascii = head + n + tail;

            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
        }
     
        public void Command(string s)
        {
            Command command = new Command(s, true, true);
            _conn.AddCommand(command);
          
        }
        public string DelayedRead()
        {
            //wait for aligner to respond. 
            Thread.Sleep(5000);
            return _conn.Read();
        }


        public void Clear()
        {
            _conn.Clear();
        }

        public void Config()
        {        
            _conn.Write("VL", false);
            int count = 0;
                       
            while (true)
            {
                string line = _conn.Read();
                
                
                if (line=="")
                {
                    int row = Console.CursorTop;
                    Console.SetCursorPosition(0, row - 1);
                    Console.WriteLine($"Read {count} lines of config");
                    break;
                }
                count += 1;
                Console.WriteLine(line);    

            }           
            
        }
    }
}
