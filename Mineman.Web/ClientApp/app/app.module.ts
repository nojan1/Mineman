import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UniversalModule } from 'angular2-universal';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { ServerListComponent } from './components/serverlist/serverlist.component';
import { ServerComponent } from './components/serverlist/server.component';
import { ImageListComponent } from './components/images/imagelist.component';
import { AddImageComponent } from './components/images/addimage.component';
import { WorldListComponent } from './components/worlds/worldlist.component';
import { AddWorldComponent } from './components/worlds/addworld.component';

import { JoinPipe } from './components/pipes/join.pipe';

import { ServerService } from './services/servers.service';
import { AuthService } from './services/auth.service';
import { ImageService } from './services/image.service';
import { ErrorService } from './services/error.service';
import { WorldService } from './services/world.service';

import { AuthModule } from './auth.module';

import { ModalModule } from 'angular2-modal';
import { BootstrapModalModule } from 'angular2-modal/plugins/bootstrap';

@NgModule({
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        ServerListComponent,
        ServerComponent,
        JoinPipe,
        ImageListComponent,
        AddImageComponent,
        WorldListComponent,
        AddWorldComponent
    ],
    providers: [
        ServerService,
        AuthService,
        ImageService,
        ErrorService,
        WorldService
    ],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'serverlist', pathMatch: 'full' },
            { path: 'serverlist', component: ServerListComponent },
            { path: 'images', component: ImageListComponent },
            { path: 'images/add', component: AddImageComponent },
            { path: 'worlds', component: WorldListComponent },
            { path: 'worlds/add', component: AddWorldComponent },
            { path: '**', redirectTo: 'serverlist' }
        ]),
        AuthModule,
        ModalModule.forRoot(),
        BootstrapModalModule
    ]
})
export class AppModule {
}
