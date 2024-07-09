# QM project README

## Classes:

Main - Intializes connections and controllers, opens ports, being user input loop

UserInputHandler - Gets user input string, modifies, and sends to contorollers

StageController - Sends ASCII commands to move X,Y,Z,T

AlignerController - Sends ASCII commands to move aligner

Connection - Serial port connection, read/writes commands, Handles Command Queue


## Description:

Console program to control X,Y,Z,T stages and aligner. 

## Commands:

All ASCII commands found is Copleys ASCII guide can be used.

End and Exit: **quit**

<ins>Stage Commands:</ins>

Home an axis: **home \<axis>**

Move by Absolute: **moveabs \<axis> \<value>**

Move by Relative: **moverel \<axis> \<value>**

move to center position: **center \<axis>**

move to positive limit position: **poslimit \<axis>**


Get Position: **pos \<axis>**

Get/Set velocity: **vel \<axis> \<value>**

Get/Set move distance: **dis \<axis> \<value>**

Get/Set acceleration: **accel \<axis> \<value>**

Get/Set deceleration: **decel \<axis> \<value>**

Reset all Amplifiers: **reset**

Read Error codes: **error \<axis>**

stop all motion: **stop**

Turn on motor: **on \<axis>**
  
Turn off motor: **off \<axis>**

<ins>joystick:</ins>

Activate Joystick Fast mode: **joyfast \<axis>**

Activate Joystick slow mode: **joyfast \<axis>**

Turn off joystick: **joyoff \<axis>**

Activate fast mode all axis: **joyonall**

deactivate all axis: **joyoffall**

joystick status: **joystatus**

<ins>solenoid valves:</ins>

Turn on Festo solenoid: **fsol \<number> on**

Turn off Festo solenoid: **fsol \<number> off**

Get Festo solenid status: **fsol \<number>**

close gripper: **grip**

open gripper: **ungrip**

toggle tilt break: **tbreak**

get tilt break status: **tbreakstatus**

<ins>routines:</ins>

Run routine Align 300mm: **alignwafer300**
  
Run routine Align 200mm: **alignwafer200**
  
Run routine Align 150mm: **alignwafer150**
  
Run routine go to wafer pick position: **pickupwafer**

<ins>vacuum:</ins>

turn on vacuum: **vacuumOn**

turn on vacuum: **vacuumOff**

vacuum status: **vacuumstatus**

<ins>aligner commands</ins>

All aligner commands are in the document: Commands.txt<br/>To use those commands type "a" in front.

raise chuck up: **a ZMU**

raise chuck up with vacuum: **a ZVMU**

Lower chuck: **a ZMD**

find notch or flat: **a APF**

<ins>other aligner commands:</ins>

rotate wafer to absolute degree: **rotatewafer \<degree>**


## Notes:

When Joystick is turned on, JoyStickLimitCheck() is run. It checks if limit is reached and turns off joystick, after 2 seconds joystick is turned on again.

After device has been unpluged it will need to home again.

BUG: JoyStickLimitCheck() on Tilt might miss. It happens when joystick speed is too high, and limit checks on other axis are running simultaneously.

There is a possibility for microscope to hit wafer if there is too much tilt when wafer is in range. It should not be a concern since tilt will only be used for probe cards and operator will watch.

Aligner has a longer readtimeout because responses are sent only after a command is completed. It is possible to change this in aligner configs. See manual

Copley ASCII Programmers guide:
- Position Mode: page 27
- Error bits are on page 45
- Getting inputs and outputs: page 58

Sequences in IMAC are set to control joystick
- 1. high speed
- 2. low speed
- 3. off

WARNING: if you turn on joystick for axis whos wires are not connected to amplifier, the motor will just move in the negative position ignoring limits!

Homeing methods are set in CME software

Input should end in **\n**. Responses end in **\r**