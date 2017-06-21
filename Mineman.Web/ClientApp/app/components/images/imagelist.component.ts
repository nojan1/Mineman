import { Component, OnInit } from '@angular/core';

import { ErrorService } from '../../services/error.service';
import { ImageService } from '../../services/image.service';
import { LoadingService } from '../../services/loading.service';

@Component({
    selector: 'imagelist',
    templateUrl: './imagelist.component.html'
})
export class ImageListComponent implements OnInit {

    constructor(private imageService: ImageService,
                private errorService: ErrorService,
                private loadingService: LoadingService) { }

    public images: any[];

    public ngOnInit() {
        this.imageService.Get()
            .catch(this.errorService.catchObservable)
            .subscribe(images => this.images = images);
    }

    public delete(event: Event, image: any) {
        var loadingHandle = this.loadingService.setLoading("Deleting image");

        this.imageService.Delete(image.id)
            .catch(this.errorService.catchObservable)
            .subscribe(() => {
                var index = this.images.indexOf(image);
                this.images.splice(index, 1);
            },
            () => { },
            () => this.loadingService.clearLoading(loadingHandle));
    }
}
