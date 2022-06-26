import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model:any = {};

  constructor(public accountService : AccountService) { }

  ngOnInit(): void {
  }

  login()
  {
    this.accountService.login(this.model).subscribe(response =>{
      console.log(response);
     // this.loggedIn = true;  
      error: (error: any) => console.log(error);    
    })    
  }

  logout(){
    //this.loggedIn = false;
    this.accountService.logout();
  }

  /*it's not good way to subscribe it, so we direct call our observale using accountService with public property 
  getCurrentUser(){
    this.accountService.currentUSer$.subscribe((user) =>{
      this.loggedIn = !!user;
      error: (error: any) => console.log(error);    
    })
  }*/
}
