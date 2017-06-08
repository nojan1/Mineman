import { NgModule } from '@angular/core';
import { Http, RequestOptions } from '@angular/http';
import { AuthHttp, AuthConfig, JwtHelper } from 'angular2-jwt';

import { AuthService } from './services/auth.service';

export function authHttpServiceFactory(http: Http, options: RequestOptions, authService: AuthService) {
    return new AuthHttp(new AuthConfig({
        tokenName: 'token',
        tokenGetter: (() => authService.GetToken()),
    }), http, options);
}

export function jwtHelperServiceFactory() {
    return new JwtHelper();
}

@NgModule({
    providers: [
        {
            provide: AuthHttp,
            useFactory: authHttpServiceFactory,
            deps: [Http, RequestOptions, AuthService]
        },
        {
            provide: JwtHelper,
            useFactory: jwtHelperServiceFactory
        }
    ]
})
export class AuthModule { }