﻿import { Component, OnInit, Input } from '@angular/core';

import { ErrorService } from '../../services/error.service';
import { AuthService } from '../../services/auth.service';
import { ServerService } from '../../services/servers.service';

@Component({
    selector: 'server',
    templateUrl: './server.component.html',
    styleUrls: ['./server.component.css']
})
export class ServerComponent implements OnInit {
    @Input() serverWithInfo: any;

    public queryFailed: boolean;

    constructor(public authService: AuthService,
                private serverService: ServerService,
                private errorService: ErrorService) { }

    ngOnInit() {
        if (this.serverWithInfo.isAlive) {
            this.serverService.queryInfo(this.serverWithInfo.id)
                .catch(this.errorService.catchObservableSilent)
                .subscribe(query => {
                    this.serverWithInfo.query = query;
                }, () => {
                    this.queryFailed = true;
                });
        }
    }
}
