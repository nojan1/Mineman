import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthHttpService {

  constructor(private auth: AuthService, private http: HttpClient) { }

  public get<T>(url: string) {
    return this.http.get<T>(url, {
      headers: this.auth.getAuthHeader()
    });
  }

  public post<T>(url: string, body: any) {
    return this.http.post<T>(url, body, {
      headers: this.auth.getAuthHeader()
    });
  }

  public delete(url: string) {
    return this.http.delete(url, {
      headers: this.auth.getAuthHeader()
    });
  }

}
