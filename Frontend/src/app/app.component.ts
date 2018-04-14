import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';

import { GLOBAL } from './services/global';
import { AnalisisService } from './services/analisis.service';
import { Analisis } from './models/analisis';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  providers: [AnalisisService]
})
export class AppComponent implements OnInit{
  public title = 'Techni';
  public analisis: Analisis[];
  public identity;
  public token;
  public errorMessage;
  public alertMessage;
  public url: string;

  constructor(
    private _route: ActivatedRoute,
    private _router: Router,
    private _analisisService: AnalisisService
  ){

    this.url = GLOBAL.url;
    this.identity = "true";
  }

  ngOnInit(){
  	//this.getAllAnalisis();
  }

  getAllAnalisis(){
    this._analisisService.getAllAnalisis().subscribe(
      response=> {
        if(!response){
          this._router.navigate(['/']);
        } else{
          //console.log(response);
            /*for (var i in response){
               console.log(response.analisis);
               //let aux = new Analisis(response[i]);
               //this.analisis.push(response.analisis[i]);
               //let aux = new Analisis(response[i]);
               //this.analisis.tristeza = "2";
            }*/

            this.analisis = response.analisis;
            


        }
        //console.log(this.analisis);
      },
      error => {
        var errorMessage = <any>error;
        if(errorMessage != null){
          var body = JSON.parse(error._body);
          this.alertMessage = body.message;

          console.log(error);
        }
      }
    );
  }

}
