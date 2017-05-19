import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { Headers } from '@angular/http';
import { AuthHttp } from 'angular2-jwt';

import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'addimage',
    templateUrl: './addimage.component.html'
})
export class AddImageComponent{

    constructor(public authService: AuthService, private authHttp: AuthHttp, private router: Router) { }

    public imageFile
    public imageAddModel = { DisplayName: "", ModDir: ""};

    fileEvent(fileInput: any) {
        this.imageFile = fileInput.target.files[0];
        console.log(this.imageFile);
    }

    public onSubmit() {
        if (this.imageFile) {
            var data = new FormData();
            data.append("DisplayName", this.imageAddModel.DisplayName);
            data.append("ModDir", this.imageAddModel.ModDir);
            data.append("ImageContents[]", this.imageFile);

            let headers = new Headers();
            headers.append("Content-Type", "multipart/form-data");

            this.authHttp.post("/api/image", data, { headers: headers })
                .subscribe(() => {
                    this.router.navigateByUrl("/images");
                });
        }
    }

}
