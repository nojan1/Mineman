import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';

import { AuthService } from './auth.service';

import { Observable } from 'rxjs';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class WorldService {
    constructor(private http: Http, private authHttp: AuthHttp, private authService: AuthService) { }

    public Get() {
        return this.authHttp.get("/api/world")
            .map(r => r.json());
    }

    public Add(displayName: string, worldFile: any) {
        if (!this.authService.isLoggedIn) {
            return Observable.throw("Not logged in");
        }

        var data = new FormData();
        data.append("DisplayName", displayName);

        if (worldFile) {
            data.append("WorldFile", worldFile);
        }

        var headers = new Headers({
            "Authorization": "Bearer " + this.authService.GetToken()
        });

        return this.http.post("/api/world", data, { headers: headers });
    }

    public GetInfo(serverId: number) {
        return this.http.get("/api/server/info/" + serverId)
            .map(r => r.json());
    }

    public GetMapInfo(serverId: number) {
        return this.http.get("/api/server/map/info/" + serverId)
            .map(r => r.json());
    }

    public Delete(worldId: number) {
        return this.authHttp.delete("/api/world/" + worldId);
    }
}