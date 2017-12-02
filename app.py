from flask import Flask
app = Flask(__name__)

@app.route("/")
def hello():
    return "Hello, Flask!"

@app.route("/control/left", methods = ['GET','POST'])
def left():
    return "it works!"

if __name__ == "__main__":
    app.run()
