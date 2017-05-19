import { Component, OnInit } from '@angular/core';

import { ServerService } from '../../services/servers.service';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'serverlist',
    templateUrl: './serverlist.component.html',
    styleUrls: ['./serverlist.component.css']
})
export class ServerListComponent implements OnInit {
    public servers: any[];

    constructor(private serverService: ServerService, public authService: AuthService) { }

    public ngOnInit() {
        this.serverService.getServers()
            .then(servers => {
                this.servers = servers

                this.servers.forEach(s => {
                    this.serverService.queryInfo(s.server.id)
                        .then(query => {
                            s.query = query;
                        });
                });
            });
    }

    public addServer(event: Event) {

    }
}
