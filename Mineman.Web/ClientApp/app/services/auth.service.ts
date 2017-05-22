import { Injectable, OnInit } from '@angular/core';
import { Headers, Http, URLSearchParams, RequestOptions } from '@angular/http';
import { AuthHttp, AuthConfig } from 'angular2-jwt';

import 'rxjs/add/operator/toPromise';

interface TokenResponse {
    access_token: string,
    expires_in: number;
}

@Injectable()
export class AuthService {
    public isLoggedIn: boolean;

    constructor(private http: Http) {
        if (this.ReadToken()) {
            this.isLoggedIn = true;
        }
    }

    public Login = (username: string, password: string) => {
        var body = new URLSearchParams();
        body.set("username", username);
        body.set("password", password);

        let headers = new Headers();
        headers.append("Content-Type", "application/x-www-form-urlencoded");

        return this.http.post("/token", body.toString(), {
            headers: headers
        }).toPromise()
        .then((raw) => {
            var token = raw.json() as TokenResponse;
            this.WriteToken(token.access_token);
            this.isLoggedIn = true;
        });
    }

    public GetToken = () => {
        if (!this.isLoggedIn)
            throw Error("Not logged in");

        return this.ReadToken();
    }

    public Logout = () => {
        this.WriteToken("");
        this.isLoggedIn = false;
    }

    private WriteToken(token: string) {
        localStorage.setItem("auth_token", token);
    }

    private ReadToken() {
        return localStorage.getItem("auth_token");
    }
}