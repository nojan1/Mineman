import { Component } from '@angular/core';

@Component({
    selector: 'serverlist',
    templateUrl: './serverlist.component.html'
})
export class ServerListComponent {
    public servers: server.serverAddModel[];// = [{motd: "Hello"}];
}
