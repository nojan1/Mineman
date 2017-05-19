import { Component, OnInit } from '@angular/core';
import { Modal } from 'angular2-modal/plugins/bootstrap';

import { ServerService } from '../../services/servers.service';
import { AuthService } from '../../services/auth.service';

import { AddImageComponent } from './addimage.component';

@Component({
    selector: 'imagelist',
    templateUrl: './imagelist.component.html'
})
export class ImageListComponent implements OnInit {

    constructor(private serverService: ServerService, public authService: AuthService, private modal: Modal){ }

    public images: any[];

    public ngOnInit() {
        this.images = [{ name: "Vanilla 1.11", modDir: "", buildStatus: "N/A" },
                       { name: "Forge 1.10.0 3567", modDir: "mods", buildStatus: "Succeeded" }];
    }

    public newImage(event: Event) {
        this.modal.alert()
            .size("lg")
            .body("<addimage></addimage>")
            .open();
    }

}
