import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'login',
    templateUrl: './login.component.html'
})
export class LoginComponent {
    constructor(private authService: AuthService,
                private router: Router) { }

    public login(event: Event, username: string, password: string) {
        this.authService.Login(username, password)
            .then(() => {
                this.router.navigateByUrl("/");
            })
            .catch(reason => {
                alert("Login failed. Reason: " + reason);
            });
    }
}
