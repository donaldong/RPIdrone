from RPIO import PWM

class Motor:
    V = 11.1
    KV = 920
    __dma_channel = 0
    __subcycle_time_us = 20000
    __pulse_incr_us = 10
    
    def __init__(self, gpio, name):
        self.__gpio = gpio
        self.name = name
        self.__servo = PWM.Servo(
            self.__dma_channel, 
            self.__subcycle_time_us, 
            self.__pulse_incr_us)
        self.__width = 0
        self.set_pulse_width(0)

    def __str__(self):
        _str = {}
        _str["name"] = self.name
        _str["pulse_width"] = self.__width
        _str["rpm"] = float(self.__width) / self.__subcycle_time_us * self.KV * self.V
        return str(_str)

    def set_pulse_width(self, us):
        self.__width = min(self.__subcycle_time_us - self.__pulse_incr_us, us)
        self.__width -= self.__width % self.__pulse_incr_us
        self.__servo.set_servo(self.__gpio, self.__width)

    def set_percentage(self, percent):
        self.set_pulse_width(percent * self.__subcycle_time_us)

    def stop(self):
        self.__servo.stop_servo(self.__gpio)

