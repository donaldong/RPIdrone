from mpu6050 import mpu6050
from collections import deque
import copy

class MPU:
    ADDRESS = 0x68
    ZERO = {"x" : 0.0, "y" : 0.0, "z" : 0.0}
    CALIBRATION = 100
    
    def __init__(self):
        self.__gyro = deque()
        self.__accel = deque()
        self.__sensor = mpu6050(self.ADDRESS)

    def gyro(self):
        data = self.__sensor.get_gyro_data() 
        if len(self.__gyro) < self.CALIBRATION:
            self.__gyro.append(data)
            return self.__average(self.__gyro)
        avg = self.__average(self.__gyro)
        self.__gyro.popleft()
        self.__gyro.append(copy.deepcopy(data))
        self.__sub(data, avg)
        return data

    def accel(self):
        data = self.__sensor.get_accel_data()
        if len(self.__accel) < self.CALIBRATION:
            self.__accel.append(data)
            return self.__average(self.__accel)
        avg = self.__average(self.__accel)
        self.__accel.popleft()
        self.__accel.append(copy.deepcopy(data))
        self.__sub(data, avg)
        return data

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

    def __average(self, data):
        res = self.__zero()
        for d in data:
            self.__add(res, d)
        res["x"] /= len(data)
        res["y"] /= len(data)
        res["z"] /= len(data)
        return res

