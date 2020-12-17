import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Role } from '../models/role.model';
import { UserEditor } from '../models/user-editor.model';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }

  deleteUser(row: UserEditor): Observable<any> {
    return this.http.delete(`api/Administration/users/${row.id}`);
  }

  updateUser(userEditor: UserEditor): Observable<any> {
    const httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'})
    }
    return this.http.put<UserEditor>(`api/Administration/users/${userEditor.id}`, JSON.stringify(userEditor), httpOptions);
  }

  newUser(userEditor: UserEditor): Observable<any> {
    const httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'})
    }
    return this.http.post<UserEditor>(`api/Administration/users`, JSON.stringify(userEditor), httpOptions);
  }

  getUsers(page: number = -1, pageSize: number = -1): Observable<any> {
    return this.http.get<User[]>(`api/Administration/users/${page}/${pageSize}`).pipe(map((data => {
      return data;
    })));
  }
  
  getRoles(): Observable<any> {
    return this.http.get<Role[]>(`api/Administration/roles`).pipe(map((data => {
      return data;
    })));
  }
}
