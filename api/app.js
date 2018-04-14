'use strict'

var express = require('express');
var bodyParser = require('body-parser');

var app = express();

// cragar rutas
var analisis_routes = require('./routes/analisis');

app.use(bodyParser.urlencoded({extended: true, limit: '50mb'}));
app.use(bodyParser.json({limit: '50mb'}));


//configurar cabeceras http
app.use((req, res, next) => {
	res.header('Access-Control-Allow-Origin', '*');
	res.header('Access-Control-Allow-Headers', 'Authorization, X-API-KEY, Origin, X-Requested-With, Content-Type, Access-Control-Allow-Request-Method');
	res.header('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, DELETE');
	res.header('Allow', 'GET, POST, OPTIONS, PUT, DELETE');

	next();
});

//rustas base
app.use('/api', analisis_routes);
	//middleware

/*app.get('/pruebas', function(req, res){
	res.status(200).send({message: 'Bienvenido al curso de Aplicaciones Web'});
})*/

module.exports = app;
