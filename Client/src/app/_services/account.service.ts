import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_modal/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseURL = environment.apiUrl;
  //own observale to store our user in 
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http : HttpClient) { }

  //use obserevable with pipe 
  login(model : any){
    return this.http.post(this.baseURL + 'account/login' , model).pipe(
      map((response: User) =>{
        const user = response;
        if(user)
        {
          this.setCurrentUser(user);
        }
      })
    );
  }

  register(model : any){
    return this.http.post(this.baseURL + 'account/register' , model).pipe(
      map((user: User) =>{
        if(user){
          this.setCurrentUser(user);
        }
        return user;
      })
    )
  }

  setCurrentUser(user : User){
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);

  }




}
