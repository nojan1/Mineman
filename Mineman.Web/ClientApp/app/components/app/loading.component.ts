import { Component, NgZone } from '@angular/core';

import { LoadingService, LoadingInstance } from '../../services/loading.service';

@Component({
    selector: 'loading',
    templateUrl: './loading.component.html',
    styleUrls: ['./loading.component.css']
})
export class LoadingComponent {

    public currentInstance: LoadingInstance;

    constructor(loadingService: LoadingService,
                zone: NgZone) {

        loadingService.instanceChanged.subscribe(instance => {
            zone.run(() => {
                this.currentInstance = instance;
            });
        });
    }
}
