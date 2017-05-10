import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import 'rxjs/add/operator/toPromise';

@Injectable()
export class ServerService {
    constructor(private http: Http) { }

    public getServers(): Promise<any[]> { //TODO: Use correct types...
        return this.http.get("/api/server")
            .toPromise()
            .then(response => response.json());

    }

    public queryInfo(serverId: number): Promise<any> {
        return this.http.get("/api/server/query/" + serverId)
            .toPromise()
            .then(response => response.json());
    }
}