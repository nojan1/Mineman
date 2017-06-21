import { Component, OnInit, Input } from '@angular/core';

import { ErrorService } from '../../services/error.service';
import { LoadingService } from '../../services/loading.service';
import { ModsService } from '../../services/mods.service';

@Component({
    selector: 'modlist',
    templateUrl: './modlist.component.html',
    //styleUrls: ['./modselector.component.css']
})
export class ModList implements OnInit {
    public mods;

    constructor(public loadingService: LoadingService,
        private modsService: ModsService,
        private errorService: ErrorService) { }

    ngOnInit() {
        var loadingHandle = this.loadingService.setLoading("Loading mods");

        this.modsService.Get()
            .catch(this.errorService.catchObservableSilent)
            .subscribe(mods => {
                this.mods = mods;
            },
            () => { },
            () => {
                this.loadingService.clearLoading(loadingHandle);
            });
    }

    public delete(event,mod) {
        var loadingHandle = this.loadingService.setLoading("Deleting mod");

        this.modsService.Delete(mod.id)
            .catch(this.errorService.catchObservable)
            .subscribe(() => {
                var index = this.mods.indexOf(mod);
                this.mods.splice(index, 1);
            },
            () => { },
            () => this.loadingService.clearLoading(loadingHandle));
    }
}
