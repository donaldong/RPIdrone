class pid:
	def _init_(p, i, d, target):
		self.kp = p
		self.ki = i
		self.kd = d
		
		self.integrator = 0.0
		self.derivator = 0.0
		
		self.err = 0.0
		self.target = target
		
	def update(data):
		# Calculate error
		self.error = self.target - data
		
		# P part of PID
		p = self.kp * self.error
		
		# I part of PID (integrals)
		self.integrator = self.integrator + self.error
		i = self.ki * self.integrator
		
		# D part of PID (derivatives)
		d = self.kd * (self.error - self.derivator)
		self.derivator = self.error
		
		# Return PID output value
		return p + i + d