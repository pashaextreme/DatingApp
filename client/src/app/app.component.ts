import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'Dotnet Application';
  users:any;
  constructor(private http:HttpClient){}

  ngOnInit(): void {
    this.http.get("http://localhost:5176/api/users").subscribe({
      next: response => this.users = response,
      error: err => console.log(err),
      complete: () => console.log("SUCCESS")      
    });
  }

}
