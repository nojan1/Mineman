import { Component, OnInit } from '@angular/core';

import { ErrorService } from '../../services/error.service';
import { ImageService } from '../../services/image.service';

@Component({
    selector: 'imagelist',
    templateUrl: './imagelist.component.html'
})
export class ImageListComponent implements OnInit {

    constructor(private imageService: ImageService, private errorService: ErrorService) { }

    public images: any[];

    public ngOnInit() {
        this.imageService.Get()
            .catch(this.errorService.catchObservable)
            .subscribe(images => this.images = images);
    }
}
