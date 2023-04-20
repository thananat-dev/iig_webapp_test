import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';


@Injectable({ providedIn: 'root' })
export class UserService {
    constructor(private http: HttpClient) { }

    getAll() {
        return this.http.get<User[]>(`${environment.apiUrl}/api/Users/`);
    }

    register(addUserRequest:any) {
        const headers = { 'Content-Type': 'application/json', };
        return this.http.post<any>(`${environment.apiUrl}/api/Users`, addUserRequest, { headers: headers,responseType:"json" })
            .pipe(map(res => {
                console.log("addUser",res);
                
                return res;
            }));
    }

    update(updateUserRequest:any) {
        const headers = { 'Content-Type': 'application/json', };
        return this.http.put<any>(`${environment.apiUrl}/api/Users`, updateUserRequest, { headers: headers,responseType:"json" })
            .pipe(map(res => {
                console.log("updateUser",res);
                
                return res;
            }));
    }

    getUserByUserId() {
        return this.http.get<any>(`${environment.apiUrl}/api/Users/getProfile`);
    }

}