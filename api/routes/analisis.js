'use strict'

var express = require('express');
var AnalisisController = require('../controllers/analisis');
var api = express.Router();

var multipart = require('connect-multiparty');
var md_upload = multipart({uploadDir: './uploads/dibujos'});

api.get('/get-analisis/:id', AnalisisController.getAnalisis);
api.post('/analisis', AnalisisController.saveAnalisis);
	api.post('/analisis-prueba', AnalisisController.seeAnalisisFromCS);
api.get('/get-all-analisis', AnalisisController.getAllAnalisis);
//api.put('/album/:id', md_auth.ensureAuth, AlbumController.updateAlbum);
api.delete('/analisis-delete/:id', AnalisisController.deleteAnalisis);
//api.post('/upload-image-album/:id', [md_auth.ensureAuth, md_upload], AlbumController.uploadImage);
api.get('/get-image-analisis/:imageFile', AnalisisController.getImageFile);

module.exports = api;