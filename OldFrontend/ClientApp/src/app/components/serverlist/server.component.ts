import { Component, OnInit, Input } from '@angular/core';

import { ErrorService } from '../../services/error.service';
import { AuthService } from '../../services/auth.service';
import { ServerService } from '../../services/servers.service';
import { catchError } from 'rxjs/operators';

@Component({
    selector: 'server',
    templateUrl: './server.component.html',
    styleUrls: ['./server.component.css']
})
export class ServerComponent implements OnInit {
    @Input() serverWithInfo: any;

    public playerNames: string[];
    public queryFailed: boolean;

    constructor(public authService: AuthService,
                private serverService: ServerService,
                private errorService: ErrorService) { }

    ngOnInit() {
        if (this.serverWithInfo.isAlive) {
            this.serverService.queryInfo(this.serverWithInfo.id)
                .pipe(catchError(this.errorService.catchObservableSilent))
                .subscribe(query => {
                    this.serverWithInfo.query = query;
                    this.playerNames = query.players.map(p => p.name);
                }, () => {
                    this.queryFailed = true;
                });
        }
    }
}
