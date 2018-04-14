import { BrowserModule } from '@angular/platform-browser';
import { ChartsModule } from 'ng2-charts';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { routing, appRoutingProviders } from './app.routing';

import { AppComponent } from './app.component';

import { HomeComponent } from './components/home.component';
import { DetallesComponent } from './components/detalles.component';
/*import { UserEditComponent } from './components/user-edit.component';
import { UserEditPasswordComponent } from './components/user-edit-password.component';
import { ProyectosComponent } from './components/proyectos-list.component';
import { ProyectoCreateComponent } from './components/create-project.component';
import { ProyectoDetallesComponent } from './components/detalles-proyecto.component';
import { MiembroAddComponent } from './components/add-miembro.component';
import { RegistrarMiembroComponent } from './components/registrar-miembro.component';
import { ProjectUpdateComponent } from './components/proyecto-update.component';
import { MiembrosGetComponent } from './components/miembros-list.component';
import { MiembroEditComponent } from './components/miembros-edit.component';*/

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    DetallesComponent
    /*UserEditComponent,
    UserEditPasswordComponent,
    ProyectosComponent,
    ProyectoCreateComponent,
    ProyectoDetallesComponent,
    MiembroAddComponent,
    RegistrarMiembroComponent,
    ProjectUpdateComponent,
    MiembrosGetComponent,
    MiembroEditComponent*/
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    ChartsModule,
    routing
  ],
  providers: [appRoutingProviders],
  bootstrap: [AppComponent]
})
export class AppModule { }
