from flask import *
from flask_api import status
from motor import Motor
from mpu import MPU
import json


class Controller(Flask):
    SUCCESS_MSG_ACCEPTED = "Success", status.HTTP_202_ACCEPTED
    ERR_MSG_MOTOR_NAME = "Invalid motor name", status.HTTP_406_NOT_ACCEPTABLE
    ERR_MSG_BAD_FORMAT = "Invalid json format", status.HTTP_406_NOT_ACCEPTABLE

    def __init__(self):
        Flask.__init__(self, "controller")
        self.__motors = {}
        self.__motors["red_cw_1"] = Motor(5, "red_cw_1")
        self.__motors["black_ccw_1"] = Motor(6, "black_ccw_1")
        self.__motors["red_cw_2"] = Motor(13, "red_cw_2")
        self.__motors["black_ccw_2"] = Motor(19, "black_ccw_2")
        self.__mpu = MPU()
        # Define control routes
        self.add_url_rule("/", view_func=self.view_root, methods=["GET"])
        self.add_url_rule("/accel", view_func=self.view_accel, methods=["GET"])
        self.add_url_rule("/gyro", view_func=self.view_gyro, methods=["GET"])
        self.add_url_rule("/motor/<string:name>", view_func=self.view_motor, methods=["GET"])
        self.add_url_rule("/speed/<string:name>", view_func=self.set_speed, methods=["GET", "POST"])

    def view_root(self):
        return Controller.SUCCESS_MSG_ACCEPTED

    def view_gyro(self):
        return json.dumps(self.__mpu.gyro())

    def view_accel(self):
        return json.dumps(self.__mpu.accel())

    def view_motor(self, name):
        try:
            return str(self.__motors[name])
        except:
            return Controller.ERR_MSG_MOTOR_NAME

    def set_speed(self, name):
        try:
            motor = self.__motors[name]
        except:
            return Controller.ERR_MSG_MOTOR_NAME
        try:
            post = json.loads(request.form.keys()[0])
            print(post)
            if "percent" in post:
                motor.set_percentage(float(post["percent"]))
                return Controller.SUCCESS_MSG_ACCEPTED
            if "pulse_width" in post:
                motor.set_pulse_width(int(post["pulse_width"]))
                return Controller.SUCCESS_MSG_ACCEPTED
            return Controller.ERR_MSG_BAD_FORMAT
        except:
            return Controller.ERR_MSG_BAD_FORMAT


if __name__ == "__main__":
    controller = Controller()
    controller.run(host="0.0.0.0", port=80)

