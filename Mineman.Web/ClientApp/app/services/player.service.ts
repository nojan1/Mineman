import { Injectable } from '@angular/core';
import { Http } from '@angular/http';

import { Observable } from 'rxjs';
import 'rxjs/add/operator/mergeMap';

export interface PlayerProfile {
    id: string,
    name: string,
    skinUrl: string
}

@Injectable()
export class PlayerService {
    constructor(private http: Http) { }

    public getProfile(uuid: string): Observable<PlayerProfile> {
        uuid = uuid.replace(/-/g, "").toLowerCase();

        return this.http.get("/api/player/" + uuid)
            .map(r => r.json());
    }
}