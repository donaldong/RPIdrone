# RPIdrone

### Authors
- [Donald Dong](https://www.linkedin.com/in/xdonaldong/), Drone Simulation
- [Christopher Forsythe](https://www.linkedin.com/in/ccforsythe/), Drone Controller
- Brandon Mendel, Drone Simulation
- Antonio Villagomez - Drone Assmenbly 
### Abstract
- Used Drone Simulation in combination with reenforcement learning to autobalance a drone
### Introduction
- RPI pin [layout](https://www.jameco.com/Jameco/workshop/circuitnotes/raspberry_pi_circuit_note_fig2a.jpg).
- Gyro + Accelerometer [MPU-6050](http://blog.bitify.co.uk/2013/11/interfacing-raspberry-pi-and-mpu-6050.html) and
    [a python module](https://github.com/Tijndagamer/mpu6050). 
    Don't forget to [enable I2C](https://www.raspberrypi.org/forums/viewtopic.php?f=28&t=97314).
- Motor [spec](https://www.robomart.com/dji-2212-920kv-brushless-motor-for-multicopter)
### Run
- `sudo python controller.py`
### Usage
- `curl -d '{"percent":0.5}' -H "Content-Type: application/json" -X POST 10.0.1.9/speed/red_cw_1`

Project Title

The Purpose of this project was to create an educational drone. 
The interest in drones has been growing over the past couple 
of years. However, this is a hobby that can be quite expensive. 
Furthermore, the learning curve into building your own could be more difficult.
Our drone is designed to be an affordable and easier learning experience.

Getting Started

In getting started you will need a good intersted and some patience. This project also tested our patience 
but we hope as you carry out this project on your own you can learn from our mistakes.

Prerequisites

What things you need to install and put together

PDB: https://www.amazon.com/gp/product/B071K945GL/ref=oh_aui_detailpage_o01_s00?ie=UTF8&psc=1

Gyro:
https://www.amazon.com/gp/product/B008BOPN40/ref=oh_aui_detailpage_o02_s00?ie=UTF8&psc=1

Propellers:
https://www.amazon.com/gp/product/B01CJMJ886/ref=oh_aui_detailpage_o01_s00?ie=UTF8&psc=1

Motors:
https://www.amazon.com/gp/product/B00URCNAV2/ref=oh_aui_detailpage_o06_s00?ie=UTF8&psc=1

ESC:
https://www.amazon.com/gp/product/B00URCO7E6/ref=oh_aui_detailpage_o06_s00?ie=UTF8&psc=1

Battery:
https://www.amazon.com/gp/product/B0072AEY5I/ref=oh_aui_detailpage_o07_s00?ie=UTF8&psc=1

For A frame its best you research as that will be based upon what you use for a controller or how you communicate to your 
drone.

For ours we used A Rasberri Pi3
Amazon has many frames that can accommodate this module and the parts listed above also all at an affordable price
---------------------------------------
TOTAL COST OF OUR DRONE: between 160-170
*Keep in mind avg drone price built and or from other tutorials goes above 350

software and how to install them

Before Flying the drone we had to simulate the flights in order to increase our success rate as well limit
damage on the drone by not flying it into a wall before knowing what it was gonna do.

For this we used UNITY to create a simulator for our drone.

https://docs.unity3d.com/ScriptReference/Physics.Simulate.html

https://unity3d.com/get-unity/download

This is free


Give examples
Installing

A step by step series of examples that tell you have to get a development env running

Say what the step will be

Give the example
And repeat

until finished
End with an example of getting some data out of the system or using it for a little demo

Running the tests

Explain how to run the automated tests for this system

Break down into end to end tests

Explain what these tests test and why

Give an example
And coding style tests

Explain what these tests test and why

Give an example
Deployment

Add additional notes about how to deploy this on a live system

Built With

We created a Web that controls the propellers speed

This was done using PHP, JAVASCRIPT.
Webgi and Flask was used in the communication between the Pi and the App

https://developer.mozilla.org/en-US/docs/Web/API/WebGL_API

http://flask.pocoo.org/

