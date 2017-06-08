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
    
}