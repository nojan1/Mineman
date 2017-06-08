import { Injectable, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Headers, Http, URLSearchParams, RequestOptions } from '@angular/http';
import { AuthHttp, AuthConfig, JwtHelper } from 'angular2-jwt';

import 'rxjs/add/operator/toPromise';

interface TokenResponse {
    access_token: string,
    expires_in: number;
}

@Injectable()
export class AuthService {
    public isLoggedIn: boolean;

    constructor(private http: Http,
                private jwtHelper: JwtHelper,
                private router: Router) {

        if (this.ReadToken() && !this.TokenExpired()) {
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
        if (!this.isLoggedIn || this.TokenExpired()) {
            console.info("Not logged in, redirecting to login page");
            this.router.navigateByUrl("/login");
        }

        return this.ReadToken();
    }

    public Logout = () => {
        this.WriteToken("");
        this.isLoggedIn = false;
    }

    private TokenExpired() {
        return this.jwtHelper.isTokenExpired(this.ReadToken());
    }

    private WriteToken(token: string) {
        localStorage.setItem("auth_token", token);
    }

    private ReadToken() {
        return localStorage.getItem("auth_token");
    }
}