'use strict'

var mongoose = require('mongoose');
var app = require('./app');
var port = process.env.PORT || 3977;
var http = require('http');

var server = http.createServer(app);
var io = require('socket.io').listen(server);


mongoose.Promise = global.Promise;
mongoose.connect('mongodb://localhost:27017/techni_hack', (err, res) => {
	if(err){
		throw err;
	}else{
		console.log("La conexion con la base de datos es correcta...");

		server.listen(port, function(){
			console.log("Servidor del api rest en http://localhost:" + port);
		});
	}
});

/**
 * Socket events
 */
io.sockets.on('connection', function(socket){
  console.log('Socket connected');
  // Socket event for gist created
  socket.on('gistSaved', function(gistSaved){
    io.emit('gistSaved', gistSaved);
  });

  // Socket event for gist updated
  socket.on('gistUpdated', function(gistUpdated){
    io.emit('gistUpdated', gistUpdated);
  });
});