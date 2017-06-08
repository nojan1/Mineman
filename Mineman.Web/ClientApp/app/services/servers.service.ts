﻿import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';

import { Observable } from 'rxjs';

import 'rxjs/add/operator/toPromise';

@Injectable()
export class ServerService {
    constructor(private http: Http, private authHttp: AuthHttp) { }

    public getServers() {
        return this.http.get("/api/server")
            .map(r => r.json());
    }

    public getSingle(id: number) {
        return this.authHttp.get("/api/server/" + id)
            .map(r => r.json());
    }

    public queryInfo(serverId: number) {
        return this.http.get("/api/server/query/" + serverId)
            .map(r => r.json().data);
    }

    public getLog(serverId: number) {
        return this.authHttp.get("/api/server/log/" + serverId)
            .map(r => r.json());
    }

    public add(serverAddModel: any) {
        return this.authHttp.post("/api/server", serverAddModel)
            .map(r => r.json());
    }

    public start(serverId: number): Observable<number> {
        return this.authHttp.post("/api/server/start/" + serverId, {})
            .map(r => r.json().result);
    }

    public stop(serverId: number): Observable<number> {
        return this.authHttp.post("/api/server/stop/" + serverId, {})
            .map(r => r.json().success ? 0 : 1);
    }

    public updateConfiguration(serverId: number, serverConfigurationModel: any) {
        return this.authHttp.post("/api/server/" + serverId, serverConfigurationModel)
            .map(r => r.json());
    }

    public rconCommand(serverId: number, command: string): Observable<string> {
        return this.authHttp.post("/api/server/rcon/" + serverId, { Command: command })
            .map(r => r.json().Response);
    }

    public destroyContainer(serverId: number) {
        return this.authHttp.delete("/api/server/destroy/" + serverId)
            .map(r => r.json().success);
    }
}