import { Injectable } from '@angular/core';

import { AuthService } from './auth.service';
import { UploadingBaseService } from './uploadingservice.base';
import { AuthHttpService } from './auth-http.service';

@Injectable()
export class ModsService extends UploadingBaseService {
    constructor(private authHttp: AuthHttpService, authService: AuthService) {
        super(authService);
    }

    public Get() {
        return this.authHttp.get<any>("/api/mod");
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