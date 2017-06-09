import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';

import { AuthService } from './auth.service';

import { Observable } from 'rxjs';

@Injectable()
export class ModsService {
    constructor(private http: Http, private authHttp: AuthHttp, private authService: AuthService) { }

    public Get() {
        return this.authHttp.get("/api/mod")
            .map(r => r.json());
    }

    public Add(displayName: string, modFile: any) {
        if (!this.authService.isLoggedIn) {
            return Observable.throw("Not logged in");
        }

        var data = new FormData();
        data.append("DisplayName", displayName);
        data.append("ModFile", modFile);

        var headers = new Headers({
            "Authorization": "Bearer " + this.authService.GetToken()
        });

        return this.http.post("/api/mod", data, { headers: headers });
    }
}