using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMProjectTektronix
{
    public class Routines
    {   
        /// <summary>
        /// Used to end routine loops. Input "stop" command to set to true.
        /// </summary>
        static public bool Stop { get; set; } = false;

        static Routines(){}

        /// <summary>
        /// performes cycles of moving to position for receiving wafer and then moving to sensor position for aligning wafer.
        /// keeps and outputs count of cycles.
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="ac"></param>
        /// <param name="size">wafer size 150/200/300</param>
        /// <returns></returns>
        static public async Task PickUpWaferAndAlignCycle(StageController sc, AlignerController ac, int size)
        {
            int count = 0;
            Stop = false;
            while(!Stop)
            {
                await WaferPickUpPosition(sc, ac);
                await AlignWafer(sc, ac, size);
                count += 1;
                Console.WriteLine("count:"+count);
            }          
        }
        /// <summary>
        /// Moves aligner to position for receiving wafer. Raises chuck, turns off vacuum, opens gripper.
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="ac"></param>
        /// <returns></returns>
        static public async Task WaferPickUpPosition(StageController sc, AlignerController ac)
        {
            try
            {
                //pick up wafer
                await sc.MoveAbsoluteAsync("x", Positions.XAlignLocation);               
                await sc.MoveAbsoluteAsync("y", Positions.Center["y"]);

                //ungrip
                await sc.Fsol(2, "on");

                //vacuum off
                ac.VacuumOff();
                await ac.WaitVacuumOff();
                
                //move chuck up
                ac.MoveUp();
                await ac.WaitForUp();

                //wait for stage to finish moving
                await sc.WaitMoveComplete("x");
                await sc.WaitMoveComplete("y");

                Console.WriteLine("ready for wafer");
            }
            catch (OperationFailedException ex)
            {
                Stop = true;
                Console.WriteLine("routine failed:" + ex.Message);
            }
        }
        /// <summary>
        /// performs alignment of wafer. Waits for wafer sensor to respond. Moves aligner to correct notch/falt sensor position (based on size). 
        /// Opens gripper. Turns off vacuum. Moves chuck with wafer down. Closes gripper to center wafer. 
        /// Turns on vacuum and moves chuck with wafer up. Alignes wafer.
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="ac"></param>
        /// <param name="size">wafer size 150/200/300</param>
        /// <returns></returns>
        static public async Task AlignWafer(StageController sc, AlignerController ac, int size)
        {
            try
            {
                await sc.WaitForWafer();

                //Go to align position
                await sc.MoveAbsoluteAsync("x", Positions.XAlignLocation);
                await sc.MoveAbsoluteAsync("y", Positions.YAlignLocations[size]);
                
                //ungrip
                await sc.Fsol(2, "on");
                await sc.WaitForUngrip();

                //turn off vacuum
                ac.VacuumOff();
                await ac.WaitVacuumOff();         

                //move wafer down
                var a = await ac.MoveDown();              
                await ac.WaitForDown();

                //center wafer (grip)
                await sc.Fsol(1, "on");              
                await sc.WaitForGrip();          

                //vacuum On
                await ac.VacuumOn();
                await ac.WaitVacuumOn();

                //Move up with vacuum
                await ac.ZVacuumUp();
                await ac.WaitForUp();

                //make sure in correct position
                await sc.WaitMoveComplete("x");
                await sc.WaitMoveComplete("y");

                //align wafer
                await ac.Align();
                await ac.WaitForAlign();

                Console.WriteLine("routine done");
            }
            catch (OperationFailedException ex)
            {
                Stop = true;
                Console.WriteLine("operation failed:" + ex.Message);
            }
        }
        
        /// <summary>
        /// Moves y stage from home to max positive position. Used for testing.
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="ac"></param>
        /// <returns></returns>
        static public async Task Routine1(StageController sc, AlignerController ac)
        {
            while (!Stop)
            {
                await sc.HomeStage("y");
                await sc.WaitMoveComplete("y");
                await sc.MoveAbsoluteAsync("y", 200000);
                await sc.WaitMoveComplete("y");
            }
            Stop = false;
        }
    }
}
