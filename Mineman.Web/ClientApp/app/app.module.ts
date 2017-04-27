import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UniversalModule } from 'angular2-universal';
import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { ServerListComponent } from './components/serverlist/serverlist.component';
import { ServerComponent } from './components/serverlist/server.component';

import { JoinPipe } from './components/pipes/join.pipe';

@NgModule({
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        ServerListComponent,
        ServerComponent,
        JoinPipe
    ],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        RouterModule.forRoot([
            { path: '', redirectTo: 'serverlist', pathMatch: 'full' },
            { path: 'serverlist', component: ServerListComponent },
            { path: '**', redirectTo: 'serverlist' }
        ])
    ]
})
export class AppModule {
}
