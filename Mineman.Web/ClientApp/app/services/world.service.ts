import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';

import { AuthService } from './auth.service';
import { UploadingBaseService } from './uploadingservice.base';

import { Observable } from 'rxjs';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class WorldService extends UploadingBaseService {
    constructor(private http: Http, private authHttp: AuthHttp, authService: AuthService) {
        super(authService);
    }

    public Get() {
        return this.authHttp.get("/api/world")
            .map(r => r.json());
    }

    public Add(displayName: string, worldFile: any) {
        var data = new FormData();
        data.append("DisplayName", displayName);

        if (worldFile) {
            data.append("WorldFile", worldFile);
        }

        return this.makeFileRequest("/api/world", data, true);
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