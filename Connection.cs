using System;
using System.Text;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace QMProjectTektronix
{
    public class Connection : SerialPort
    {

        private string ComPortName = "";              
        private bool Opened = false;
        public int ReadWriteTime;
        byte Delimiter = (byte)'\r';
        byte[] leftover;

        BlockingCollection<Command> commandQueue = new BlockingCollection<Command>();

        public Connection(int ComPortNum, int Baud, int RWTime) : base()
        {
            base.BaudRate = Baud;
            base.Parity = Parity.None;
            base.DataBits = 8;
            base.StopBits = StopBits.One;

            ReadWriteTime = RWTime;

            base.ReadTimeout = 1000;
            base.WriteTimeout = 1000;
            
            ComPortName = "COM" + ComPortNum;
            Console.WriteLine($"intializing at {ComPortName}");                   
        }

        public void OpenPort()
        {
            try
            {
                base.PortName = ComPortName;
                base.Open();
                Opened = true;
                Console.WriteLine($"connection opened at {ComPortName}");                
                base.BaseStream.ReadTimeout = ReadWriteTime;
                base.BaseStream.WriteTimeout = ReadWriteTime;
                var consumeQueueTask = Task.Run(() => StartConsumingLoop());
                TestPort();
                //LoopReading();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error: Port {0} is in use", ComPortName);
            }
            catch
            {
                Console.WriteLine("Error: could not connect to Port {0}", ComPortName);              
            }
        }

        public async Task TestPort()
        {
            var command = new Command("0", true, false);
            AddCommand(command);
            string res = await command.TSC.Task;
            if (res==null)
            {
                Console.WriteLine($"Could not communicate with device on {ComPortName}. Check that it is connected and turned on");
                End();
            }
        }

        public async Task StartConsumingLoop()
        {           
            foreach (var command in commandQueue.GetConsumingEnumerable())
            {                               
                await ProcessCommand(command);
                //await Task.Delay(100);
            }                   
        }
        
        public void AddCommand(Command command)
        {         
            commandQueue.Add(command);
        }

        //Writes, Reads, and Outputs commands
        public async Task ProcessCommand(Command command)
        {          
            string res = "";
            Write(command.Ascii,false);
            if (command.Read)
            {
                res = await Task.Run(() => ReadBytes());
                if (command.Display)
                {
                    Console.WriteLine("response: " + res);
                }
                command.TSC.SetResult(res);
            }           
        }       

        /* 
        * read until nothing to read
        */
        public void Clear()
        {
            string res = Read();
            if (res=="")
            {
                Console.WriteLine("output cleared");
            }
            else
            {
                Clear();
            }
        }

        //write string
        public string Write(string code, bool read = true)
        {          
            try
            {
                base.WriteLine(code);
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("connection busy - could not write");
            }
            catch (InvalidOperationException)
            {

            }       
            catch (TimeoutException)
            {              
                Console.WriteLine("Write Timed Out");
            }           
            return read ? Read() : null;
        }

        //write bytes
        public string Write(byte[] code, bool read = true)
        {
            try
            {
                base.BaseStream.Write(code, 0, 10);
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("connection busy - could not write");
            }
            catch (InvalidOperationException)
            {

            }
            catch (TimeoutException)
            {
                Console.WriteLine("Write Timed Out");
            }
            return read ? Read() : null;
        }

        public string Read()
        {
            string res = "";
            try
            {
                res = base.ReadTo("\r");
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("connection busy - could not read");
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Read Timed Out");
            }
            return res;
        }

        public void ReadingLoop()
        {
            if (Opened==false)
            {
                return;
            }
            byte[] buffer = new byte[1];
            base.BaseStream.BeginRead(buffer, 0, buffer.Length, delegate (IAsyncResult ar) {                              
                    try
                    {
                        int actualLength = base.BaseStream.EndRead(ar);
                        byte[] received = new byte[actualLength];
                        Buffer.BlockCopy(buffer, 0, received, 0, actualLength);
                        raiseAppSerialDataEvent(received);
                    }
                    catch (System.IO.IOException exc)
                    {
                        //handleAppSerialError(exc);
                    }
                    catch (InvalidOperationException)
                    {

                    }                                    
                    ReadingLoop();                          
            }, null);
        }

        public void raiseAppSerialDataEvent(byte[] received)
        {
            
            int newlineIndex = Array.IndexOf(received, Delimiter, 0);
            if (newlineIndex < 0)
            {
                leftover = ConcatArray(leftover, received, 0, received.Length - 0);
            }
            else
            {
                byte[] full_line = ConcatArray(leftover, received, 0, 1);
                leftover = null;
                //string res = Encoding.ASCII.GetString(full_line);
                //return res;
                ProcessFullLine(full_line);
            }
        }

        public string ProcessFullLine(byte[] line)
        {
            string res = Encoding.ASCII.GetString(line);
            //NewFullLine?.Invoke(line);
            Console.WriteLine(res);
            return res;
        }

        public string ReadBytes()
        {            
            if (Opened==false)
            {
                return "closed port";
            }
            int pos = 0;
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1];                   
                    int nread = base.BaseStream.Read(buffer, pos, 1);                 

                    int newlineIndex = Array.IndexOf(buffer, Delimiter, 0);
                    if(newlineIndex<0)
                    {
                        leftover = ConcatArray(leftover, buffer, 0, buffer.Length - 0);
                    }
                    else
                    {
                        byte[] full_line = ConcatArray(leftover, buffer, 0, 1);
                        leftover = null;
                        string res = Encoding.ASCII.GetString(full_line);
                        return res;
                    }
                }
                catch (TimeoutException)
                {
                    Console.WriteLine($"Read Timed Out on port {ComPortName}");
                    return null;
                }
                catch (System.IO.IOException)
                {                   
                    Console.WriteLine("The requested resource is in use");
                    return null;

                }
                catch (InvalidOperationException)
                {
                    //trys to read after port has closed after quit command
                    Console.WriteLine("port is closed, could not read");
                    return null;
                }
            }
        }

        public byte[] ConcatArray(byte[] head, byte[] tail, int tailOffset, int tailCount)
        {
            byte[] result;
            if (head == null)
            {
                result = new byte[tailCount];
                Array.Copy(tail, tailOffset, result, 0, tailCount);
            }
            else
            {
                result = new byte[head.Length + tailCount];
                head.CopyTo(result, 0);
                Array.Copy(tail, tailOffset, result, head.Length, tailCount);
            }

            return result;
        }

        public void End()
        {
            while(commandQueue.Count>0)
            {
                Thread.Sleep(100);
            }
            commandQueue.CompleteAdding();
            Opened = false;
            base.Close();
        }

        public string WriteRead(string code)
        {
            Write(code); 
            return Read();
        }       
    }
}
