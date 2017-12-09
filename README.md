# RPIdrone
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
