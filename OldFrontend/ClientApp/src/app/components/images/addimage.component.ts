import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { ImageService } from '../../services/image.service';
import { LoadingService } from '../../services/loading.service';
import { ErrorService } from '../../services/error.service';
import { catchError } from 'rxjs/operators';

@Component({
    selector: 'addimage',
    templateUrl: './addimage.component.html'
})
export class AddImageComponent{

    constructor(public errorService: ErrorService,
                private loadingService: LoadingService,
                private imageService: ImageService,
                private router: Router) { }

    public imageFile
    public imageAddModel = { DisplayName: "", ModDir: ""};

    fileEvent(fileInput: any) {
        this.imageFile = fileInput.target.files[0];
    }

    public onSubmit() {
        if (this.imageFile) {
            var loadingHandle = this.loadingService.setLoading("Adding image");

            this.imageService.progress.subscribe(progress => this.loadingService.updateProgress(loadingHandle, progress));

            this.imageService.Add(this.imageAddModel.DisplayName, this.imageAddModel.ModDir, this.imageFile)
                .pipe(catchError(this.errorService.catchObservable))
                .subscribe(
                    () => this.router.navigateByUrl("/images"),
                    () => { },
                    () => this.loadingService.clearLoading(loadingHandle)
                );
        }
    }

}
