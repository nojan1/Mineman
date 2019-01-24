import { Injectable, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';

interface TokenResponse {
    access_token: string,
    expires_in: number;
}

@Injectable()
export class AuthService {
    public isLoggedIn: boolean;

    constructor(private http: HttpClient,
        private router: Router) {

        if (this.ReadToken() && !this.TokenExpired()) {
            this.isLoggedIn = true;
        }
    }

    public Login = (username: string, password: string) => {
        var body = new URLSearchParams();
        body.set("username", username);
        body.set("password", password);

        let headers = new HttpHeaders();
        headers = headers.set("Content-Type", "application/x-www-form-urlencoded");

        var observable = this.http.post("/token", body.toString(), {
            headers: headers
        });

        observable.subscribe((token: TokenResponse) => {
            //var token = raw.json() as TokenResponse;
            this.WriteToken(token.access_token);
            this.isLoggedIn = true;
        });

        return observable;
    }

    public GetToken = () => {
        if (!this.isLoggedIn || this.TokenExpired()) {
            console.info("Not logged in, redirecting to login page");
            this.router.navigateByUrl("/login");
        }

        return this.ReadToken();
    }

    public getAuthHeader = () =>  {
        return new HttpHeaders().set("Authorization", `Bearer ${this.GetToken()}`)
    };

    public Logout = () => {
        this.WriteToken("");
        this.isLoggedIn = false;
    }

    private TokenExpired() {
        let jwtHelper = new JwtHelperService();
        return jwtHelper.isTokenExpired(this.ReadToken());
    }

    private WriteToken(token: string) {
        localStorage.setItem("auth_token", token);
    }

    private ReadToken() {
        return localStorage.getItem("auth_token");
    }
}