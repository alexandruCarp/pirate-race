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


# TODO

# Prima pagina in unity, in care se alege nr de jucatori, cand se apasa new game se face socketio.emit catre server (cu nr
# de jucatori si codul)
# Urmatoarea pagina in unity va afisa cati jucatori s au conectat (asteapta sa primeasca socketio.emit de la server 
# ca sa modifice numarul)

# Serverul afiseaza pagina initiala in care se introduce codul ca pagina de start (in '/')
# Cand se apasa submit, serverul verifica codul si numarul de jucatori deja conectati. Daca e ok, redirectioneaza spre pagina
# de alegere barca etc (momentan doar o pagina cu un text care zice ca s a conectat cu succes), altfel afiseaza un text de eroare
# pe pagina de start
