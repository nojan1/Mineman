import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { ErrorService } from '../../services/error.service';
import { ServerService } from '../../services/servers.service';
import { ImageService } from '../../services/image.service';
import { WorldService } from '../../services/world.service';

@Component({
    selector: 'addserver',
    templateUrl: './addserver.component.html'
})
export class AddServerComponent implements OnInit {

    constructor(private serverService: ServerService,
        private errorService: ErrorService,
        private imageService: ImageService,
        private worldService: WorldService,
        private router: Router) { }

    public serverAddModel = { Description: "", WorldID: -1, ImageID: -1, ServerPort: 25565, MemoryAllocationMB: 1024};

    public images: any[];
    public worlds: any[];

    public ngOnInit() {
        this.worldService.Get()
            .subscribe(x => this.worlds = x);

        this.imageService.Get()
            .subscribe(x => this.images = x);
    }

    public onSubmit() {
        this.serverService.add(this.serverAddModel)
            .catch(this.errorService.catchObservable)
            .subscribe(() => this.router.navigateByUrl("/serverlist"));
    }
}
