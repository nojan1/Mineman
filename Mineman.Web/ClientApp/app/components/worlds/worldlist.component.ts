import { Component, OnInit } from '@angular/core';

import { ErrorService } from '../../services/error.service';
import { WorldService } from '../../services/world.service';

@Component({
    selector: 'worldlist',
    templateUrl: './worldlist.component.html'
})
export class WorldListComponent implements OnInit {

    public worlds: any[];

    constructor(private worldService: WorldService, private errorService: ErrorService) { }

    public ngOnInit() {
        this.worldService.Get()
            .catch(this.errorService.catchObservable)
            .subscribe(x => this.worlds = x);
    }
}
