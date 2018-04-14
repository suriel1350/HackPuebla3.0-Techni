'use strict'

var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var AnalisisSchema = Schema({
	anger: Number,
	contempt: Number,
	disgust: Number, 	
	fear: Number,
	joy: Number,
	sadness: Number,
	surprice: Number,
	tiempo: String,
	genero: String,
	image: String
	//artist: { type: Schema.ObjectId, ref: 'Artist' }
});

module.exports = mongoose.model('Analisis', AnalisisSchema);