import { Component, OnInit } from '@angular/core';

import { ErrorService } from '../../services/error.service';
import { WorldService } from '../../services/world.service';
import { LoadingService } from '../../services/loading.service';
import { catchError } from 'rxjs/operators';

@Component({
    selector: 'worldlist',
    templateUrl: './worldlist.component.html'
})
export class WorldListComponent implements OnInit {

    public worlds: any[];

    constructor(private worldService: WorldService,
                private errorService: ErrorService,
                private loadingService: LoadingService) { }

    public ngOnInit() {
        this.worldService.Get()
            .pipe(catchError(this.errorService.catchObservable))
            .subscribe(x => this.worlds = x);
    }

    public delete(event: Event, world: any) {
        var loadingHandle = this.loadingService.setLoading("Deleting world");

        this.worldService.Delete(world.id)
            .pipe(catchError(this.errorService.catchObservable))
            .subscribe(() => {
                var index = this.worlds.indexOf(world);
                this.worlds.splice(index, 1);
            },
            () => { },
            () => this.loadingService.clearLoading(loadingHandle));
    }
}
