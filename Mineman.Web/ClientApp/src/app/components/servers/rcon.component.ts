import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ErrorService } from '../../services/error.service';
import { ServerService } from '../../services/servers.service';
import { LoadingService } from '../../services/loading.service';

@Component({
    selector: 'rcon',
    templateUrl: './rcon.component.html',
    styleUrls: ['./rcon.component.css']
})
export class RconComponent implements OnInit {

    public serverId: number;
    public responses = [];

    constructor(private serverService: ServerService,
        private errorService: ErrorService,
        private route: ActivatedRoute,
        private loadingService: LoadingService) { }

    public ngOnInit() {
        this.route.params.subscribe(params => {
            this.serverId = +params["id"];
        });
    }

    public sendCommand(event: Event, command: string) {
        this.serverService.rconCommand(this.serverId, command)
            .subscribe(response => {
                this.insertResponse(response);
            },
            error => {
                this.insertResponse("Error! " + error);
            },
            () => { });
    }

    private insertResponse(response: string) {
        this.responses.unshift(response);
    }
}
