import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

export interface PlayerProfile {
    id: string,
    name: string,
    skinUrl: string
}

@Injectable()
export class PlayerService {
    constructor(private http: HttpClient) { }

    public getProfile(uuid: string): Observable<PlayerProfile> {
        uuid = uuid.replace(/-/g, "").toLowerCase();

        return this.http.get<PlayerProfile>("/api/player/" + uuid);
            //.map(r => r.json());
    }
}