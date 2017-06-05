import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ErrorService } from '../../services/error.service';
import { ServerService } from '../../services/servers.service';
import { LoadingService } from '../../services/loading.service';

import { ToastsManager } from 'ng2-toastr/ng2-toastr';

@Component({
    selector: 'logviewer',
    templateUrl: './logviewer.component.html',
    styleUrls: ['./logviewer.component.css']
})
export class LogViewerComponent implements OnInit {

    public serverId: number;
    public logData: string;

    constructor(private serverService: ServerService,
        private errorService: ErrorService,
        private route: ActivatedRoute,
        private toastr: ToastsManager,
        private loadingService: LoadingService) { }

    public ngOnInit() {
        this.route.params.subscribe(params => {
            this.serverId = +params["id"];
            this.loadLogData();
        });
    }

    private loadLogData() {
        let loadingHandle = this.loadingService.setLoading("Loading serverdetails");

        this.serverService.getLog(this.serverId)
            .catch(this.errorService.catchObservable)
            .subscribe(data => this.logData = data,
            () => { },
            () => {
                this.loadingService.clearLoading(loadingHandle);
            });
    }
}
