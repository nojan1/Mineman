import { Observable } from 'rxjs';

import { AuthService } from './auth.service';
import { share } from 'rxjs/operators';

export class UploadingBaseService {
    public progress: Observable<number>;
    private progressObserver;

    constructor(private authService: AuthService) {
        this.progress = new Observable<number>(observer => {
            this.progressObserver = observer
        }).pipe(share());
    }

    protected makeFileRequest(url: string, formData: FormData, authRequest: boolean): Observable<any> {
        return new Observable(observer => {
            let xhr: XMLHttpRequest = new XMLHttpRequest();
            
            xhr.onreadystatechange = () => {
                if (xhr.readyState === 4) {
                    if (xhr.status === 200) {
                        observer.next(JSON.parse(xhr.response));
                        observer.complete();
                    } else {
                        observer.error(xhr.response);
                    }
                }
            };

            xhr.upload.onprogress = (event) => {
                let progress = Math.round(event.loaded / event.total * 100);

                this.progressObserver.next(progress);
            };

            xhr.open('POST', url, true);

            if (authRequest) {
                if (!this.authService.isLoggedIn) {
                    observer.error("Not logged in");
                }

                xhr.setRequestHeader("Authorization", "Bearer " + this.authService.GetToken());
            }

            xhr.send(formData);
        });
    }
}