import { Component, OnInit, Input } from '@angular/core';

import { ErrorService } from '../../services/error.service';
import { AuthService } from '../../services/auth.service';
import { ModsService } from '../../services/mods.service';
import { catchError } from 'rxjs/operators';

@Component({
    selector: 'modselector',
    templateUrl: './modselector.component.html',
    styleUrls: ['./modselector.component.css']
})
export class ModSelector implements OnInit {
    @Input() modIds: number[];

    public availableMods;
    public modLoadingComplete = false;

    constructor(public authService: AuthService,
        private modsService: ModsService,
        private errorService: ErrorService) { }

    ngOnInit() {
        this.modsService.Get()
            .pipe(catchError(this.errorService.catchObservableSilent))
            .subscribe(mods => {
                this.availableMods = mods;
            },
            () => { },
            () => {
                this.modLoadingComplete = true;
            });
    }

    public notSelectedMods() {
        return this.availableMods.filter(m => this.modIds.indexOf(m.id) == -1);
    }

    public selectedMods() {
        return this.availableMods.filter(m => this.modIds.indexOf(m.id) != -1);
    }

    public addMod(mod: any) {
        if (this.modIds.indexOf(mod.id) == -1) {
            this.modIds.push(mod.id);
        }
    }

    public removeMod(mod: any) {
        var index = this.modIds.indexOf(mod.id);
        if (index == -1)
            return;

        this.modIds.splice(index, 1);
    }
}
