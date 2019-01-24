import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { ErrorService } from '../../services/error.service';
import { LoadingService } from '../../services/loading.service';
import { ModsService } from '../../services/mods.service';
import { catchError } from 'rxjs/operators';

@Component({
    selector: 'addmod',
    templateUrl: './addmod.component.html'
})
export class AddMod implements OnInit {

    public modFile
    public modAddModel = { DisplayName: "" };

    constructor(public loadingService: LoadingService,
        private modsService: ModsService,
        private errorService: ErrorService,
        private router: Router) { }

    ngOnInit() {

    }

    fileEvent(fileInput: any) {
        this.modFile = fileInput.target.files[0];
    }

    public onSubmit() {
        if (this.modFile) {
            var loadingHandle = this.loadingService.setLoading("Adding mod");

            this.modsService.progress.subscribe(progress => this.loadingService.updateProgress(loadingHandle, progress));

            this.modsService.Add(this.modAddModel.DisplayName, this.modFile)
                .pipe(catchError(this.errorService.catchObservable))
                .subscribe(mod => {
                    this.router.navigateByUrl("/mods");
                },
                () => { },
                () => {
                    this.loadingService.clearLoading(loadingHandle);
                });
        }
    }
}
