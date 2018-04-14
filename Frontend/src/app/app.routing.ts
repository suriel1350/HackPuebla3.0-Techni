import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from  '@angular/router';

//import user
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

const appRoutes: Routes = [
	{path: '', component: HomeComponent}, 
	{path: 'inicio', component: HomeComponent},
  	{path: 'detalles-analisis/:id', component: DetallesComponent},
	/*{path: 'mis-datos', component: UserEditComponent},
	{path: 'cambiar-password', component: UserEditPasswordComponent},
	{path: 'mis-proyectos/:idUser', component: ProyectosComponent},
	{path: 'proyecto/crear/:idUser', component: ProyectoCreateComponent},
	{path: 'detalles-proyecto/:idProject', component: ProyectoDetallesComponent},
	{path: 'agregar-miembro/:idProject', component: MiembroAddComponent},
	{path: 'update-project/:idProject', component: ProjectUpdateComponent},
	{path: 'registrar-miembro/:idUser', component: RegistrarMiembroComponent},
	{path: 'editar-miembro/:idUser', component: MiembroEditComponent},
	{path: 'ver-miembros', component: MiembrosGetComponent},*/
	{path: '**', component: HomeComponent},
];

export const appRoutingProviders: any[] = [];
export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);
