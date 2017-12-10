from mpu6050 import mpu6050
import copy

class MPU:
    ADDRESS = 0x68
    ZERO = {"x" : 0.0, "y" : 0.0, "z" : 0.0}
    CALIBRATION = 100
    
    def __init__(self):
        self.__gyro_offset = self.__zero()
        self.__gyro_count = 0
        self.__accel_offset = self.__zero()
        self.__accel_count = 0 
        self.__sensor = mpu6050(self.ADDRESS)

    def gyro(self):
        data = self.__sensor.get_gyro_data() 
        if self.__gyro_count < self.CALIBRATION:
            self.__add(self.__gyro_offset, data)
            self.__gyro_count += 1
            if self.__gyro_count == self.CALIBRATION:
                self.__div(self.__gyro_offset, self.CALIBRATION)
            return self.ZERO
        self.__sub(data, self.__gyro_offset) 
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

    @staticmethod
    def __div(a, b):
        a["x"] /= b
        a["y"] /= b
        a["z"] /= b
