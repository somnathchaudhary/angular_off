import { Component, OnInit } from '@angular/core';
import { NgForm,FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  ngForm = FormsModule
  constructor() { }

  ngOnInit() {
  }

  register() {
    console.log(this.model);
  }
  cancel() {
    console.log('cancelled');
  }

}
