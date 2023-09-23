import { Component, OnInit } from '@angular/core';
import { FlightService } from '../api/services/flight.service';
import { FlightRm } from '../api/models';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';



@Component({
  selector: 'app-search-flights',
  templateUrl: './search-flights.component.html',
  styleUrls: ['./search-flights.component.css']
})
export class SearchFlightsComponent implements OnInit {
  ngOnInit(): void {
    this.search();
    }

  constructor(private flightService: FlightService,
    private fb: FormBuilder) {
  }

  searchForm = this.fb.group({
    fromDate: [''] ,
    toDate: [''],
    from: [''],
    destination: [''],
    numberOfPassengers: [1]
  });


  searchResult: FlightRm[] = []
  search() {
    this.flightService.searchFlight(this.searchForm.value as { fromDate: string; toDate: string; from: string; destination: string; numberOfPassengers: number }
    ).subscribe((response: FlightRm[]) => this.searchResult = response, this.handleError);
  }

  private handleError(err: any) {
    console.log("Respawn Error. Status : " , err.status);
    console.log("Respawn Error. Status Text : ", err.statusText);
    console.log(err)
  }

  
    
}

