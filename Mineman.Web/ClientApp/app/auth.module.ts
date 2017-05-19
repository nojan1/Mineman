import { NgModule } from '@angular/core';
import { Http, RequestOptions } from '@angular/http';
import { AuthHttp, AuthConfig } from 'angular2-jwt';

import { AuthService } from './services/auth.service';

export function authHttpServiceFactory(http: Http, options: RequestOptions, authService: AuthService) {
    return new AuthHttp(new AuthConfig({
        tokenName: 'token',
        tokenGetter: (() => authService.GetToken()),
        globalHeaders: [/*{ 'Content-Type': 'application/json' }*/],
    }), http, options);
}

@NgModule({
    providers: [
        {
            provide: AuthHttp,
            useFactory: authHttpServiceFactory,
            deps: [Http, RequestOptions, AuthService]
        }
    ]
})
export class AuthModule { }