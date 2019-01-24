import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Modal } from 'ngx-modialog/plugins/bootstrap';

import { WorldService } from '../../services/world.service';
import { ImageService } from '../../services/image.service';
import { ErrorService } from '../../services/error.service';
import { ServerService } from '../../services/servers.service';
import { LoadingService } from '../../services/loading.service';

import { catchError } from 'rxjs/operators';

@Component({
    selector: 'serverdetails',
    templateUrl: './serverdetails.component.html'
})
export class ServerDetailsComponent implements OnInit {

    public worlds: any[];
    public images: any[];
    public serverId: number;
    public serverConfigurationModel;
    public isAlive;
    public supportsMods;

    public userProperties = [];
    public propertyValues = {};

    constructor(private serverService: ServerService,
        private errorService: ErrorService,
        private route: ActivatedRoute,
        private worldService: WorldService,
        private imageService: ImageService,
        private loadingService: LoadingService,
        vcRef: ViewContainerRef,
        private modal: Modal) {
    }

    public ngOnInit() {
        this.route.params.subscribe(params => {
            this.serverId = +params["id"];
            this.loadServer();
        });

        this.worldService.Get()
            .subscribe(x => this.worlds = x);

        this.imageService.Get()
            .subscribe(x => this.images = x);
    }

    private loadServer() {
        let loadingHandle = this.loadingService.setLoading("Loading serverdetails");

        this.serverService.getSingle(this.serverId)
            .pipe(catchError(this.errorService.catchObservable))
            .subscribe(x => {
                this.isAlive = x.isAlive;
                this.userProperties = x.properties.map(s =>
                    s.replace(/__/g, ".")
                        .replace(/_/g, "-")
                        .toLowerCase());

                this.serverConfigurationModel = {
                    Description: x.server.description,
                    ServerPort: x.server.mainPort,
                    WorldID: x.server.world.id,
                    ImageID: x.server.image.id,
                    MemoryAllocationMB: x.server.memoryAllocationMB,
                    ModIds: x.server.mods.map(m => m.id),
                    Properties: this.parseServerProperties(x.server.serializedProperties)
                };

                this.supportsMods = x.server.image.supportsMods;
            },
                () => { },
                () => {
                    this.loadingService.clearLoading(loadingHandle);
                }
            );
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

        var loadingHandle = this.loadingService.setLoading(running ? "Starting server" : "Stopping server");

        result
            .pipe(catchError(this.errorService.catchObservable))
            .subscribe((result) => {
                if (result == 0) {
                    this.loadServer();
                } else if (result == 2) {
                    this.modal.alert()
                        .message("It was not possible to perform the action at this time. Underlying resources not yet ready, will be automaticly performed later")
                        .open();
                } else {
                    this.modal.alert()
                        .message("Failed to change server run status")
                        .open();
                }
            },
                () => { },
                () => {
                    this.loadingService.clearLoading(loadingHandle);
                });
    }

    public saveChanges() {
        var loadingHandle = this.loadingService.setLoading("Saving changes");

        this.serverService.updateConfiguration(this.serverId, this.serverConfigurationModel)
            .pipe(catchError(this.errorService.catchObservable))
            .subscribe(() => {
                //this.toastr.info("Server configuration saved successfully");
                this.loadServer();
            },
                () => { },
                () => {
                    this.loadingService.clearLoading(loadingHandle);
                }
            );
    }

    public destroyContainer() {
        this.modal.confirm()
            .title("Please confirm")
            .message("Are you sure you wish to destoy the container?")
            .open()
            .result.then(val => {
                var loadingHandle = this.loadingService.setLoading("Destroying container");

                this.serverService.destroyContainer(this.serverId)
                    .pipe(catchError(this.errorService.catchObservable))
                    .subscribe((success) => {
                        if (success) {
                            //this.toastr.info("Container destroyed");
                            this.loadServer();
                        } else {
                            this.modal.alert()
                                .title("Error")
                                .message("Container could not be destroyed")
                                .open();
                        }
                    },
                        () => { },
                        () => {
                            this.loadingService.clearLoading(loadingHandle);
                        });
            });
    }
}
