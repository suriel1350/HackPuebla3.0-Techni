import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/operator/map';
import { Observable } from 'rxjs/Observable';
import { GLOBAL } from './global';
import { Proyecto } from '../models/proyecto';

@Injectable()
export class ProyectoService{
	public url: string;

	constructor(private _http: Http){
		this.url = GLOBAL.url;
	}

	getMiembro(token, id){
		let headers = new Headers({
			'Content-Type':'application/json',
			'Authorization':token
		});

		let options = new RequestOptions({ headers: headers });
		return this._http.get(this.url+'miembro/'+id, options).map(res => res.json());
	}

	getProyectos(token, id){
		let headers = new Headers({
			'Content-Type':'application/json',
			'Authorization':token
		});

		let options = new RequestOptions({ headers: headers });
		return this._http.get(this.url+'proyectos/'+id, options).map(res => res.json());
	}

	getProyecto(token, id){
		let headers = new Headers({
			'Content-Type':'application/json',
			'Authorization':token
		});

		let options = new RequestOptions({ headers: headers });
		return this._http.get(this.url+'proyecto/get/'+id, options).map(res => res.json());
	}

	getMyProjects(token, id){
		let headers = new Headers({
			'Content-Type':'application/json',
			'Authorization':token
		});

		let options = new RequestOptions({ headers: headers });
		return this._http.get(this.url+'proyectos-inscrito/'+id, options).map(res => res.json());
	}

	deleteProject(token, id){
		let headers = new Headers({
			'Content-Type':'application/json',
			'Authorization':token
		});

		let options = new RequestOptions({ headers: headers });
		return this._http.delete(this.url+'proyecto/eliminar/'+id, options).map(res => res.json());
	}

	deleteMiembro(token, idPro, idMie){
		let headers = new Headers({
			'Content-Type':'application/json',
			'Authorization':token
		});

		let options = new RequestOptions({ headers: headers });
		return this._http.delete(this.url+'proyecto/eliminar-miembro/'+idPro+'/'+idMie, options).map(res => res.json());
	}

	addProject(token, project: Proyecto, id){
		let params = JSON.stringify(project);

		let headers = new Headers({
			'Content-Type':'application/json',
			'Authorization':token
		});

		return this._http.post(this.url+'proyecto/' + id + '/crear', params, {headers: headers})
						 .map(res => res.json());	
	}

	addMiembros(token, detalles_pro){
		let params = JSON.stringify(detalles_pro);

		let headers = new Headers({
			'Content-Type':'application/json',
			'Authorization':token
		});

		return this._http.post(this.url+'proyecto/agregar-miembros', params, {headers: headers})
						 .map(res => res.json());	
	}

	updateProyecto(token, id, project_to_update){
		let params = JSON.stringify(project_to_update);

		let headers = new Headers({
				'Content-Type':'application/json',
				'Authorization': token
			});

		return this._http.put(this.url+'update-project/'+id, params, {headers: headers}).map(res => res.json());	
	}
}