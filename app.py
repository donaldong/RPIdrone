from flask import Flask
from motor import Motor
from mpu6050 import mpu6050


motor_r1 = Motor(5)
motor_b1 = Motor(6)
motor_r2 = Motor(13)
motor_b2 = Motor(19)
app = Flask(__name__)


@app.route("/mpu", methods = ['GET','POST'])
def mpu():
    accel_data = sensor.get_accel_data()
    gyro_data = sensor.get_gyro_data()
    return "Acc: {}\nGyro: {}\n".format(accel_data, gyro_data)


@app.route("/control/left", methods = ['GET','POST'])
def left():
    return "it works!"


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=80)

