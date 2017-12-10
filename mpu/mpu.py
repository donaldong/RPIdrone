from mpu6050 import mpu6050

class MPU:
    ADDRESS = 0x68
    ZERO = {"x" : 0.0, "y" : 0.0, "z" : 0.0}
    CALIBRATION = 100
    
    def __init__(self):
        self.__gyro_offset = self.ZERO
        self.__gyro_count = 0
        self.__accel_offset = self.ZERO
        self.__accel_count = 0
        self.__sensor = mpu6050(self.ADDRESS)

    def gyro(self):
        data = self.__sensor.get_gyro_data() 
        if self.__gyro_count < self.CALIBRATION:
            self.__gyro_offset["x"] += data["x"]
            self.__gyro_offset["y"] += data["y"]
            self.__gyro_offset["z"] += data["z"]
            self.__gyro_count += 1
            return self.ZERO
        data["x"] -= self.__gyro_offset["x"] / self.CALIBRATION
        data["y"] -= self.__gyro_offset["y"] / self.CALIBRATION
        data["z"] -= self.__gyro_offset["z"] / self.CALIBRATION
        return data

    def accel(self):
        return self.__sensor.get_accel_data()
