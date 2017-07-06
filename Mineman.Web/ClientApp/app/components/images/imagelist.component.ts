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
    public remoteImages: any[];

    public ngOnInit() {
        this.imageService.Get()
            .catch(this.errorService.catchObservable)
            .subscribe(images => this.images = images);


        this.imageService.GetRemote()
            .catch(this.errorService.catchObservable)
            .subscribe(remoteImages => this.remoteImages = remoteImages);
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

    public addRemote(event: Event, remoteImage: any) {
        var loadingHandle = this.loadingService.setLoading("Adding remote image");

        this.imageService.AddRemote(remoteImage.shA256Hash)
            .catch(this.errorService.catchObservable)
            .subscribe(() => {
                this.ngOnInit();
            },
            () => { },
            () => this.loadingService.clearLoading(loadingHandle));
    }

    public gotLocalImage(remoteImage: any) {
        if (!this.images)
            return false;

        return this.images.some(i => i.name == remoteImage.displayName); //TODO: Change to use hash
    }
}
