from RPIO import PWM

class Motor:
    __dma_channel = 0
    __subcycle_time_us = 20000
    __pulse_incr_us = 10
    __servo = PWM.Servo(__dma_channel, __subcycle_time_us, __pulse_incr_us)
    
    def __init__(self, gpio):
        self.__gpio = gpio
        self.__servo = PWM.Servo(self.__dma_channel, self.__subcycle_time_us, self.__pulse_incr_us)

    def set_pulse_width(self, us):
        self.__servo.set_servo(self.__gpio, us)

    def stop(self):
        self.__servo.stop_servo(self.__gpio)

