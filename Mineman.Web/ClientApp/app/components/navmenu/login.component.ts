import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { AuthService } from '../../services/auth.service';
import { LoadingService } from '../../services/loading.service';

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent {
    constructor(private authService: AuthService,
        private router: Router,
        private loadingService: LoadingService) { }

    public login(event: Event, username: string, password: string) {
        var loadingHandle = this.loadingService.setLoading("Logging in");

        this.authService.Login(username, password)
            .subscribe(() => {
                this.router.navigateByUrl("/");
            },
            (reason) => {
                alert("Login failed. Reason: " + reason);
            },
            () => { this.loadingService.clearLoading(loadingHandle); });
    }
}
