import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';

import { AuthService } from './auth.service';

import { Observable } from 'rxjs';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ImageService {
    constructor(private http: Http, private authHttp: AuthHttp, private authService: AuthService) { }

    public Get() {
        return this.authHttp.get("/api/image")
            .map(r => r.json());
    }

    public Add(displayName: string, modDirectory: string, imageFile: any) {
        if (!this.authService.isLoggedIn) {
            return Observable.throw("Not logged in");
        }

        var data = new FormData();
        data.append("DisplayName", displayName);
        data.append("ModDir", modDirectory);
        data.append("ImageContents", imageFile);

        var headers = new Headers({
            "Authorization": "Bearer " + this.authService.GetToken()
        });

        return this.http.post("/api/image", data, { headers: headers });
    }

    public Delete(imageId: number) {
        return this.authHttp.delete("/api/image/" + imageId);
    }
}