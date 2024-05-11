# app.py

from flask import Flask, request, render_template
from flask_socketio import SocketIO, emit
import socket

app = Flask(__name__)
socketio = SocketIO(app)

players_number = -1
players_connected = 0
join_code = ""


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
    
@app.route('/choose_boat')
def choose_boat():
    return render_template('choose_boat.html')


if __name__ == '__main__':
    socketio.run(app, host=socket.gethostbyname(socket.gethostname()), port=5000)


# TODO

# Prima pagina in unity, in care se alege nr de jucatori, cand se apasa new game se face socketio.emit catre server (cu nr
# de jucatori si codul) -> DONE
# Urmatoarea pagina in unity va afisa cati jucatori s au conectat (asteapta sa primeasca socketio.emit de la server 
# ca sa modifice numarul) -> DONE

# Serverul afiseaza pagina initiala in care se introduce codul ca pagina de start (in '/') -> DONE

# Cand se apasa submit, serverul verifica codul si numarul de jucatori deja conectati. Daca e ok, redirectioneaza spre pagina
# de alegere barca etc (momentan doar o pagina cu un text care zice ca s a conectat cu succes), altfel afiseaza un text de eroare
# pe pagina de start -> DONE