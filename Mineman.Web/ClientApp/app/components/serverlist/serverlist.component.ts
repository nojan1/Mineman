import { Component, OnInit } from '@angular/core';

import { ServerService } from '../../services/servers.service';
import { AuthService } from '../../services/auth.service';
import { ErrorService } from '../../services/error.service';
import { LoadingService } from '../../services/loading.service';

@Component({
    selector: 'serverlist',
    templateUrl: './serverlist.component.html',
    styleUrls: ['./serverlist.component.css']
})
export class ServerListComponent implements OnInit {
    public servers: any[];

    constructor(private serverService: ServerService,
        public authService: AuthService,
        private errorService: ErrorService,
        private loadingService: LoadingService) { }

    public ngOnInit() {
        let loadingHandle = this.loadingService.setLoading("Loading servers");

        this.serverService.getServers()
            .catch(this.errorService.catchObservable)
            .subscribe(servers => {
                this.servers = servers

                this.servers.forEach(s => {
                    if (s.isAlive) {
                        this.serverService.queryInfo(s.id)
                            .catch(this.errorService.catchObservableSilent)
                            .subscribe(query => {
                                s.query = query;
                            });
                    }
                });
            },
            () => { },
            () => {
                this.loadingService.clearLoading(loadingHandle);
            }
        );
    }
}
