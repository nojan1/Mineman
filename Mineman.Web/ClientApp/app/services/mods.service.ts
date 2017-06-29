import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';

import { AuthService } from './auth.service';
import { UploadingBaseService } from './uploadingservice.base';

import { Observable } from 'rxjs';

@Injectable()
export class ModsService extends UploadingBaseService {
    constructor(private http: Http, private authHttp: AuthHttp, authService: AuthService) {
        super(authService);
    }

    public Get() {
        return this.authHttp.get("/api/mod")
            .map(r => r.json());
    }

    public Add(displayName: string, modFile: any) {
        var data = new FormData();
        data.append("DisplayName", displayName);
        data.append("ModFile", modFile);

        return this.makeFileRequest("/api/mod", data, true);
    }

    public Delete(modId: number) {
        return this.authHttp.delete("/api/mod/" + modId);
    }
}