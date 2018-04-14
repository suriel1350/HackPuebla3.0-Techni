import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';

import { GLOBAL } from '../services/global';
import { AnalisisService } from '../services/analisis.service';
import { Analisis } from '../models/analisis';

@Component({
  selector: 'app-verDetalles',
  templateUrl: '../views/app-verDetalles.html',
  providers: [AnalisisService]
})
export class DetallesComponent implements OnInit{
  public title = 'Techni';
  public analisis: Analisis;
  public identity;
  public token;
  public errorMessage;
  public alertMessage;
  public url: string;

  public radarChartLabels:string[] = ['Alegria', 'Tristeza', 'Miedo', 'Desagrado', 'Ira', 'Disgusto', 'Sorpresa'];
  
  public radarChartData:any = [
    {data: [0, 1, 2, 3, 4, 3], label: 'Emociones'}
  ];

  public radarChartType:string = 'radar';

  public chartClicked(e:any):void {
    console.log(e);
  }

  public chartHovered(e:any):void {
      console.log(e);
    } 

  constructor(
    private _route: ActivatedRoute,
    private _router: Router,
    private _analisisService: AnalisisService
  ){
    this.identity = 'detalles';
    this.url = GLOBAL.url;
    this.analisis = new Analisis('',0,0,0,0,0,0,0,'',0,0);         
    
  }

  ngOnInit(){
    console.log("detalles.component cargado");
    this.getDetalles();
  }

  getDetalles(){
    this._route.params.forEach((params: Params) => {
      let analisisId = params['id'];      
      this._analisisService.getAnalisis(analisisId).subscribe(
        response=> {
          if(!response){
            this._router.navigate(['/']);
          } else{            
              console.log(response.analisis.image);               
            this.analisis.id = response.analisis._id;
              this.analisis.disgusto = response.analisis.joy*100;
              this.analisis.tristeza = response.analisis.sadness*100;            
              this.analisis.miedo = response.analisis.fear*100;
              this.analisis.asco = response.analisis.contempt*100;
              this.analisis.ira = response.analisis.anger*100;
              this.analisis.alegria = response.analisis.disgust*100;
              this.analisis.sorpresa = response.analisis.surprice*100;
            this.analisis.tiempo = response.analisis.tiempo;
            console.log(response.analisis.tiempo);
            this.analisis.genero = response.analisis.genero*100;
            this.analisis.image = response.analisis.image;


                 let _dataSets:Array<any> = new Array(this.radarChartData.length);

                 for (let i = 0; i < this.radarChartData.length; i++) {
                    _dataSets[i] = {data: new Array(this.radarChartData[i].data.length), label: this.radarChartData[i].label};
                    //for (let j = 0; j < this.radarChartData[i].data.length; j++) {
                      
                      //_dataSets[i].data[j] = this.radarChartData[i].data[j];
                      _dataSets[i].data[0] = this.analisis.alegria;
                      _dataSets[i].data[1] = this.analisis.tristeza;
                      _dataSets[i].data[2] = this.analisis.miedo;
                      _dataSets[i].data[3] = this.analisis.asco;
                      _dataSets[i].data[4] = this.analisis.ira;
                      _dataSets[i].data[5] = this.analisis.disgusto;
                      _dataSets[i].data[6] = this.analisis.sorpresa;

                    
                 }
                 this.radarChartData = _dataSets;            
          }
          
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
    });
  }  
}
