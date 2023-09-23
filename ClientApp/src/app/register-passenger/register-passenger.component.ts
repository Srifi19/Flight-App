import { Component, OnInit } from '@angular/core';
import { PassengerService } from './../api/services/passenger.service'
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../auth/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
@Component({
  selector: 'app-register-passenger',
  templateUrl: './register-passenger.component.html',
  styleUrls: ['./register-passenger.component.css']
})
export class RegisterPassengerComponent implements OnInit {
  ngOnInit(): void {
    this.activateRoute.params.subscribe(p => this.requestedUrl = p['requestedUrl']);
    }
  form = this.fb.group({
    email: ['', Validators.compose([Validators.required, Validators.email, Validators.minLength(6), Validators.maxLength(64)])],
    firstName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(32)])],
    lastName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(32)])],
    isFemale: [true, Validators.required]
  })
  constructor(private passengerService: PassengerService,
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private activateRoute: ActivatedRoute
  ) { }

  requestedUrl?: string = undefined;

  public checkPassenger() : void {
    const params = { email: this.form.get('email')?.value as string} ;
    this.passengerService
      .findPassenger(params)
      .subscribe(this.login, e => {
        if (e.status != 404) {
          console.error(e);
        }
      })
  }
  register() {
    console.log("form Value", this.form.value);

    this.passengerService.registerPassenger({ body: this.form.value })
      .subscribe(this.login)  ;

  }

  private login = () => {
    this.authService.loginUser({ email: this.form.get('email')?.value as string });
    this.router.navigate([this.requestedUrl ?? '/search-flights']);
  }
 


}
