import { UploadingBaseService } from './uploadingservice.base';

import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';

import { AuthService } from './auth.service';

import { Observable } from 'rxjs';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ImageService extends UploadingBaseService {
    constructor(private http: Http, private authHttp: AuthHttp, authService: AuthService) {
        super(authService);
    }

    public Get() {
        return this.authHttp.get("/api/image")
            .map(r => r.json());
    }

    public Add(displayName: string, modDirectory: string, imageFile: any) {
        var data = new FormData();
        data.append("DisplayName", displayName);
        data.append("ModDir", modDirectory);
        data.append("ImageContents", imageFile);

        return this.makeFileRequest("/api/image", data, true);
    }

    public Delete(imageId: number) {
        return this.authHttp.delete("/api/image/" + imageId);
    }

    public GetRemote() {
        return this.authHttp.get("/api/image/remote")
            .map(r => r.json());
    }

    public AddRemote(hash: string) {
        return this.authHttp.post("/api/image/remote/" + hash, null)
            .map(r => r.json());
    }
}