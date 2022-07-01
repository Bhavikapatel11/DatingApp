import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  
  constructor(private route : Router, private toastr : ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if(error) {
          switch (error.status) {
            // its for 400 validation
            case 400:
              if(error.error.errors) {
                const modalStateErrors = [];
                for(const key in error.error.errors){
                  if(error.error.errors[key]){
                  //idea of flatten the array of errors that we get from the validation push them into array
                  modalStateErrors.push(error.error.errors[key]);
                  }
                }
                throw modalStateErrors.flat();
              }
              else{
                this.toastr.error(error.error ,error.status);
              }
              break;
            
            case 401:
              this.toastr.error(error.statusText === 'OK' ? 'Unauthorised': error.statusText ,error.status);
              break;
            
            case 404:
              this.route.navigateByUrl('/not-found');
              break;

            case 500:
              //get the details of error that we going to get returned from api
              const navigationExtras: NavigationExtras = {state: {error: error.error}}
              this.route.navigateByUrl('/server-error', navigationExtras);
              break;
            
            default:
              this.toastr.error('Somethig is wrong');
              console.log(error);              
              break;
          }
        }
        return throwError(error);
      })
    );
  }
}
