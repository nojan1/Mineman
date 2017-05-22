import { Component, OnInit } from '@angular/core';

import { ImageService } from '../../services/image.service';


@Component({
    selector: 'imagelist',
    templateUrl: './imagelist.component.html'
})
export class ImageListComponent implements OnInit {

    constructor(private imageService: ImageService){ }

    public images: any[];

    public ngOnInit() {
        this.imageService.Get()
            .subscribe(images => this.images = images);
    }
}
