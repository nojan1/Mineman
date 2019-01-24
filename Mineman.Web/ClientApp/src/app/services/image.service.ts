import { UploadingBaseService } from './uploadingservice.base';

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { AuthHttpService } from './auth-http.service';

import { map } from 'rxjs/operators';

@Injectable()
export class ImageService extends UploadingBaseService {
    constructor(private http: HttpClient, authService: AuthService, private authHttp: AuthHttpService) {
        super(authService);
    }

    public Get() {
        return this.authHttp.get<any>("/api/image");
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
        return this.authHttp.get<any>("/api/image/remote");
            //.map(r => r.json());
    }

    public AddRemote(hash: string) {
        return this.authHttp.post<any>("/api/image/remote/" + hash, null);
            //.map(r => r.json());
    }
}