using System;
using System.Threading;
using System.Threading.Tasks;


namespace QMProjectTektronix
{
    public class Program
    {
        static Connection stageConnection= new Connection(4, 9600, 2000);
        static Connection alignerConnection = new Connection(6, 19200, 6000);

        static StageController stageControls = new StageController(stageConnection);
        static AlignerController alignerContorls = new AlignerController(alignerConnection);

        static UserInputHandler UserInputHandler = new UserInputHandler(stageControls, alignerContorls);

        public static bool End = false;
        
        public static void Main()
        {
            
            stageConnection.OpenPort();
            
            stageControls.SetupMotors();
            alignerConnection.OpenPort();
            //alignerConnection.ReadingLoop();              

            //main program loop
            Console.Write($"Command:");
            while (!End)
            {
                string input = Console.ReadLine();

                UserInputHandler.UserCommand(input);              
            }
            EndProcedure();
        }

        public static void EndProcedure()
        {
            stageControls.End();
            alignerContorls.End();
        }
              

    }   
}
