import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { WorldService } from '../../services/world.service';
import { ErrorService } from '../../services/error.service';
import { ServerService } from '../../services/servers.service';


@Component({
    selector: 'serverdetails',
    templateUrl: './serverdetails.component.html'
})
export class ServerDetailsComponent implements OnInit {

    public worlds: any[];
    public serverId: number;
    public serverConfigurationModel;
    public isAlive;

    public userProperties = [];
    public propertyValues = {};

    constructor(private serverService: ServerService,
        private errorService: ErrorService,
        private route: ActivatedRoute,
        private worldService: WorldService) { }

    public ngOnInit() {
        this.route.params.subscribe(params => {
            this.serverId = +params["id"];
            this.loadServer();
        });

        this.worldService.Get()
            .subscribe(x => this.worlds = x);
    }

    private loadServer() {
        this.serverService.getSingle(this.serverId)
            .catch(this.errorService.catchObservable)
            .subscribe(x => {
                this.isAlive = x.isAlive;
                this.userProperties = x.properties.map(s => 
                    s.replace(/__/g, ".")
                        .replace(/_/g, "-")
                        .toLowerCase());

                this.serverConfigurationModel = {
                    Description: x.server.description,
                    ServerPort: x.server.mainPort,
                    WorldID: x.server.worldId,
                    MemoryAllocationMB: x.server.memoryAllocationMB,
                    Properties: this.parseServerProperties(x.server.serializedProperties)
                };
            });
    }

    private parseServerProperties(rawData: string) {
        var retVal = {};
        rawData.split("\n").forEach(value => {
            var parts = value.split("=");

            retVal[parts[0]] = parts[1];
        });

        return retVal;
    }

    public setServerRunningState(running: boolean) {
        var result = running ? this.serverService.start(this.serverId)
                             : this.serverService.stop(this.serverId);

        result
            .catch(this.errorService.catchObservable)
            .subscribe((success) => {
                if (success) {
                    this.loadServer();
                } else {
                    alert("Failed to change server run status");
                }
            });
    }

    public saveChanges() {
        this.serverService.updateConfiguration(this.serverId, this.serverConfigurationModel)
            .catch(this.errorService.catchObservable)
            .subscribe(() => {
                alert("Saved. TODO: Remove me!");
            });
    }
}
