
from flask import Flask, request, render_template, make_response
from flask_socketio import SocketIO, emit
import socket

app = Flask(__name__)
socketio = SocketIO(app)

players_number = -1
players_connected = 0
players_ready = 0
join_code = ""

players_id = 0
race_in_progress = False

@app.route('/index')
def index():
    return render_template('index.html')

@socketio.on('join_code')
def handle_join_code(data):
    global join_code
    join_code = int(data)
    print('received join code: ' + str(join_code))

@socketio.on('players_number')
def handle_players_number(data):
    global players_number
    print('received players number: ' + str(data))
    players_number = int(data)

@app.route('/')
def join_game():
    return render_template('join_game.html')

@app.route('/code_submitted', methods=['POST'])
def handle_code_submission():
    global join_code
    global players_number
    global players_connected
    code = int(request.json['code'])

    print("received code: " + str(code))
    print("players number: " + str(players_number))

    # TODO: Verify the code and number of players
    if code == join_code and players_connected < players_number:
        print('valid player connected')
        players_connected += 1
        socketio.emit('new_connection', "")
        return {'valid': True, 'message': ''}
    else:
        if code != join_code:
            print('invalid code')
            return {'valid': False, 'message': 'invalid code'}
        else:
            print('too many players')
            return {'valid': False, 'message': 'too many players'}
        
@app.route('/player_ready', methods=['POST'])
def handle_player_ready():
    global players_ready
    global race_in_progress
    players_ready += 1
    name = request.json['name']
    boat_id = request.json['boatId']
    print(name + ' is ready with boat ' + str(boat_id))

    if players_ready == players_number:
        socketio.emit('all_ready', "")
        race_in_progress = True

    return {'ok': True}
    
@app.route('/choose_boat')
def choose_boat():
    player_id = request.cookies.get('player_id')
    resp = make_response(render_template('choose_boat.html'))
    if not player_id:
        global players_id
        player_id = players_id
        players_id += 1
        resp.set_cookie('player_id', str(player_id))
    return resp

@app.route('/boat_control')
def boat_control():
    return render_template('boat_control.html')

@app.route('/control_button_pressed', methods=['POST'])
def control_button_pressed():
    global race_in_progress
    button_type = request.json['button']
    player_id = request.cookies.get('player_id')

    if race_in_progress:
        socketio.emit('button_pressed', {'player_id': player_id, 'button': button_type})
    print('player ' + player_id + ' pressed ' + button_type)
    return {'ok': True}


if __name__ == '__main__':
    socketio.run(app, host=socket.gethostbyname(socket.gethostname()), port=5000)

