# app.py

from flask import Flask, request, render_template
from flask_socketio import SocketIO, emit

app = Flask(__name__)
socketio = SocketIO(app)

@app.route('/')
def index():
    return render_template('index.html')

@app.route('/button_clicked', methods=['POST'])
def button_clicked():
    data = request.json
    button = data['button']
    print(button + ' was clicked!')
    socketio.emit('buttonPressed', button)
    return '', 200

if __name__ == '__main__':
    socketio.run(app, host='192.168.1.5', port=5000)
