import { NgModule, LOCALE_ID } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

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
import { MapViewerComponent } from './components/mapviewer/mapviewer.component';
import { MapWindowComponent } from './components/mapviewer/mapwindow.component';

import { JoinPipe } from './components/pipes/join.pipe';
import { NormalizedPropertyNamePipe } from './components/pipes/normalizedpropertyname.pipe';
import { Nl2br } from './components/pipes/nl2br.pipe';
import { TruncatePipe } from './components/pipes/truncate.pipe';
import { BuildStatusFormatter } from './components/pipes/buildstatusformatter.pipe';

import { ServerService } from './services/servers.service';
import { AuthService } from './services/auth.service';
import { AuthGuardService } from './services/auth-guard.service';
import { ImageService } from './services/image.service';
import { ErrorService } from './services/error.service';
import { WorldService } from './services/world.service';
import { LoadingService } from './services/loading.service';
import { ModsService } from './services/mods.service';
import { LayoutService } from './services/layout.service';
import { PlayerService } from './services/player.service';

import { ModalModule } from 'ngx-modialog';
import { BootstrapModalModule } from 'ngx-modialog/plugins/bootstrap';

@NgModule({
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        ServerListComponent,
        ServerComponent,
        JoinPipe,
        NormalizedPropertyNamePipe,
        Nl2br,
        TruncatePipe,
        BuildStatusFormatter,
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
        AddMod,
        MapViewerComponent,
        MapWindowComponent
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
        ModsService,
        LayoutService,
        PlayerService
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
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
            { path: 'map/:id', component: MapWindowComponent },
            { path: 'login', component: LoginComponent },
            { path: '**', redirectTo: 'serverlist' }
        ]),
        ModalModule.forRoot(),
        BootstrapModalModule
    ]
})
export class AppModule {
}
