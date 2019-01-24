import { Injectable } from '@angular/core';

import { AuthService } from './auth.service';
import { UploadingBaseService } from './uploadingservice.base';

import { AuthHttpService } from './auth-http.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class WorldService extends UploadingBaseService {
    constructor(private http: HttpClient, private authHttp: AuthHttpService, authService: AuthService) {
        super(authService);
    }

    public Get() {
        return this.authHttp.get<any[]>("/api/world");
            //.map(r => r.json());
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
        return this.http.get<any>("/api/server/info/" + serverId);
    }

    public GetMapInfo(serverId: number) {
        return this.http.get<any>("/api/server/map/info/" + serverId);
    }

    public Delete(worldId: number) {
        return this.authHttp.delete("/api/world/" + worldId);
    }
}