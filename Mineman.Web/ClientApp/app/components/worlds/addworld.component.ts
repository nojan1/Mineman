import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { ErrorService } from '../../services/error.service';
import { LoadingService } from '../../services/loading.service';
import { WorldService } from '../../services/world.service';

@Component({
    selector: 'addworld',
    templateUrl: './addworld.component.html'
})
export class AddWorldComponent {

    constructor(private router: Router,
        private worldService: WorldService,
        private errorService: ErrorService,
        private loadingService: LoadingService) { }

    public worldFile
    public worldAddModel = { DisplayName: "" };

    fileEvent(fileInput: any) {
        this.worldFile = fileInput.target.files[0];
    }

    public onSubmit() {
        var loadingHandle = this.loadingService.setLoading("Adding mod");

        this.worldService.progress.subscribe(progress => this.loadingService.updateProgress(loadingHandle, progress));

        this.worldService.Add(this.worldAddModel.DisplayName, this.worldFile)
            .catch(this.errorService.catchObservable)
            .subscribe(
            () => this.router.navigateByUrl("/worlds"),
            () => { },
            () => this.loadingService.clearLoading(loadingHandle)
            );
    }
}
