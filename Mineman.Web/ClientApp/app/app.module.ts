import { NgModule, LOCALE_ID } from '@angular/core';
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
import { AddServerComponent } from './components/servers/addserver.component';
import { ServerDetailsComponent } from './components/servers/serverdetails.component';
import { PropertiesEditorComponent } from './components/servers/propertieseditor.component';
import { LoginComponent } from './components/navmenu/login.component';
import { LogViewerComponent } from './components/servers/logviewer.component';
import { LoadingComponent } from './components/app/loading.component';
import { LoadingInlineComponent } from './components/app/loading-inline.component';
import { RconComponent } from './components/servers/rcon.component';
import { ModSelector } from './components/mods/modselector.component';
import { ModList } from './components/mods/modlist.component';
import { AddMod } from './components/mods/addmod.component';

import { JoinPipe } from './components/pipes/join.pipe';
import { NormalizedPropertyNamePipe } from './components/pipes/normalizedpropertyname.pipe';
import { LogFormattingPipe } from './components/pipes/logformatting.pipe';
import { TruncatePipe } from './components/pipes/truncate.pipe';

import { ServerService } from './services/servers.service';
import { AuthService } from './services/auth.service';
import { AuthGuardService } from './services/auth-guard.service';
import { ImageService } from './services/image.service';
import { ErrorService } from './services/error.service';
import { WorldService } from './services/world.service';
import { LoadingService } from './services/loading.service';
import { ModsService } from './services/mods.service';

import { AuthModule } from './auth.module';

import { ModalModule } from 'angular2-modal';
import { BootstrapModalModule } from 'angular2-modal/plugins/bootstrap';
import { ToastModule } from 'ng2-toastr/ng2-toastr';

@NgModule({
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        ServerListComponent,
        ServerComponent,
        JoinPipe,
        NormalizedPropertyNamePipe,
        LogFormattingPipe,
        TruncatePipe,
        ImageListComponent,
        AddImageComponent,
        WorldListComponent,
        AddWorldComponent,
        AddServerComponent,
        ServerDetailsComponent,
        PropertiesEditorComponent,
        LoginComponent,
        LogViewerComponent,
        LoadingComponent,
        RconComponent,
        LoadingInlineComponent,
        ModSelector,
        ModList,
        AddMod
    ],
    providers: [
        { provide: LOCALE_ID, useValue: "sv-SE" },
        ServerService,
        AuthService,
        ImageService,
        ErrorService,
        WorldService,
        LoadingService,
        AuthGuardService,
        ModsService
    ],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'serverlist', pathMatch: 'full' },
            { path: 'serverlist', component: ServerListComponent },
            { path: 'images', component: ImageListComponent, canActivate: [AuthGuardService] },
            { path: 'images/add', component: AddImageComponent, canActivate: [AuthGuardService] },
            { path: 'worlds', component: WorldListComponent, canActivate: [AuthGuardService] },
            { path: 'worlds/add', component: AddWorldComponent, canActivate: [AuthGuardService] },
            { path: 'server/add', component: AddServerComponent, canActivate: [AuthGuardService] },
            { path: 'server/:id', component: ServerDetailsComponent, canActivate: [AuthGuardService] },
            { path: 'server/log/:id', component: LogViewerComponent, canActivate: [AuthGuardService] },
            { path: 'server/rcon/:id', component: RconComponent, canActivate: [AuthGuardService] },
            { path: 'mods', component: ModList, canActivate: [AuthGuardService] },
            { path: 'mods/add', component: AddMod, canActivate: [AuthGuardService] },
            { path: 'login', component: LoginComponent },
            { path: '**', redirectTo: 'serverlist' }
        ]),
        AuthModule,
        ModalModule.forRoot(),
        BootstrapModalModule,
        ToastModule.forRoot()
    ]
})
export class AppModule {
}
