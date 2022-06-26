import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model:any = {};

  constructor(public accountService : AccountService,
              private route : Router,
              private toastr : ToastrService) { }

  ngOnInit(): void {
  }

  login()
  {
    this.accountService.login(this.model).subscribe(response =>{
      this.route.navigateByUrl('/members');
      // this.loggedIn = true;  
    },(error: any) => {
      console.log(error);
      this.toastr.error(error.error);
    }  
    )    
  }

  showSuccess() {
    this.toastr.success('Hello world!', 'Toastr fun!');
  }

  logout(){
    //this.loggedIn = false;
    this.accountService.logout();
    this.route.navigateByUrl('/')
  }

  /*it's not good way to subscribe it, so we direct call our observale using accountService with public property 
  getCurrentUser(){
    this.accountService.currentUSer$.subscribe((user) =>{
      this.loggedIn = !!user;
      error: (error: any) => console.log(error);    
    })
  }*/
}
