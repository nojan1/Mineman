import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { ImageService } from '../../services/image.service';
import { ErrorService } from '../../services/error.service';

@Component({
    selector: 'addimage',
    templateUrl: './addimage.component.html'
})
export class AddImageComponent{

    constructor(public errorService: ErrorService, private imageService: ImageService, private router: Router) { }

    public imageFile
    public imageAddModel = { DisplayName: "", ModDir: ""};

    fileEvent(fileInput: any) {
        this.imageFile = fileInput.target.files[0];
    }

    public onSubmit() {
        if (this.imageFile) {
            this.imageService.Add(this.imageAddModel.DisplayName, this.imageAddModel.ModDir, this.imageFile)
                .catch(this.errorService.catchObservable)
                .subscribe(() => this.router.navigateByUrl("/images"))
        }
    }

}
