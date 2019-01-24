import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

import { AuthHttpService } from './auth-http.service';

@Injectable()
export class ServerService {
    constructor(private http: HttpClient, private authHttp: AuthHttpService) { }

    public getServers() {
        return this.http.get<any[]>("/api/server");
    }

    public getSingle(id: number) {
        return this.authHttp.get<any>("/api/server/" + id);
    }

    public queryInfo(serverId: number) {
        return this.http.get<any>("/api/server/query/" + serverId)
            .pipe(map(r => r.data));
    }

    public getLog(serverId: number) {
        return this.authHttp.get<any>("/api/server/log/" + serverId);
    }

    public add(serverAddModel: any) {
        return this.authHttp.post("/api/server", serverAddModel);
    }

    public start(serverId: number): Observable<number> {
        return this.authHttp.post<any>("/api/server/start/" + serverId, {})
            .pipe(map(r => r.result));
    }

    public stop(serverId: number): Observable<number> {
        return this.authHttp.post<any>("/api/server/stop/" + serverId, {})
            .pipe(map(r => r.success ? 0 : 1));
    }

    public updateConfiguration(serverId: number, serverConfigurationModel: any) {
        return this.authHttp.post<any>("/api/server/" + serverId, serverConfigurationModel);
    }

    public rconCommand(serverId: number, command: string): Observable<string> {
        return this.authHttp.post<any>("/api/server/rcon/" + serverId, { Command: command })
            .pipe(map(r => r.Response));
    }

    public destroyContainer(serverId: number) {
        return this.authHttp.delete("/api/server/destroy/" + serverId)
            .pipe(map((r: any) => r.success));
    }
}