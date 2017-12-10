from mpu6050 import mpu6050
import thread
import time
import copy

class MPU:
    ADDRESS = 0x68
    ZERO = {"x" : 0.0, "y" : 0.0, "z" : 0.0}
    CALIBRATION = 200
    DELTA_TIME = 0.01
    
    def __init__(self):
        self.__gyro_offset = self.__zero()
        self.__accel_offset = self.__zero()
        self.__gyro = self.__zero()
        self.__accel = self.__zero()
        self.__step = 0 
        self.__sensor = mpu6050(self.ADDRESS)
        thread.start_new_thread(self.update, ())

    def update(self):
        while True:
            time.sleep(self.DELTA_TIME)
            gyro_data = self.__sensor.get_gyro_data() 
            accel_data = self.__sensor.get_accel_data()
            if self.__step < self.CALIBRATION:
                self.__add(self.__gyro_offset, gyro_data)
                self.__add(self.__accel_offset, accel_data)
                self.__step += 1
                if self.__step == self.CALIBRATION:
                    self.__div(self.__gyro_offset, self.CALIBRATION)
                    self.__div(self.__accel_offset, self.CALIBRATION)
                continue
            self.__sub(gyro_data, self.__gyro_offset)
            self.__sub(accel_data, self.__accel_offset)
            self.__mul(gyro_data, self.DELTA_TIME * 2)
            self.__mul(accel_data, self.DELTA_TIME * 2)
            self.__add(self.__gyro, gyro_data)
            self.__add(self.__accel, accel_data)

    def gyro(self):
        return self.__gyro

    def accel(self):
        return self.__accel

    @staticmethod
    def __zero():
        return copy.deepcopy(MPU.ZERO)

    @staticmethod
    def __add(a, b):
        a["x"] += b["x"]
        a["y"] += b["y"]
        a["z"] += b["z"]

    @staticmethod
    def __sub(a, b):
        a["x"] -= b["x"]
        a["y"] -= b["y"]
        a["z"] -= b["z"]

    @staticmethod
    def __div(a, b):
        a["x"] /= b
        a["y"] /= b
        a["z"] /= b

    @staticmethod
    def __mul(a, b):
        a["x"] *= b
        a["y"] *= b
        a["z"] *= b
