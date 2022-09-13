import { Component, Injector, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AdvanceSelectionScreen, DropdownOutputDto, StateServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-fc-geography',
  templateUrl: './fc-geography.component.html'
})

export class FcGeographyComponent extends AppComponentBase implements OnInit {
  isLoading: boolean = true;
  states: DropdownOutputDto[] = [];
  cities: DropdownOutputDto[] = [];
  selectedState: string;
  selectedCities: string[] = [];
  popupData: string;
  searchText: string = "";

  @Input() databaseId: number;
  @Input() buildId: number;

  constructor(injector: Injector, public activeModal: NgbActiveModal, private _stateServiceProxy: StateServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getState();
    if (this.popupData) {
      this.selectedCities = this.popupData.split(',');
    }
  }
  getState() {
    this.isLoading = true;
    this._stateServiceProxy
      .getState(this.databaseId, this.buildId, AdvanceSelectionScreen.CountyCity)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(result => {
        this.states = result.states;
      });
  }

  getCity() {
    this.isLoading = true;
    this._stateServiceProxy
      .getCity(
        this.selectedState,
        undefined,
        0
      )
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(result => {
        this.cities = result.sort((a, b) => a.label.localeCompare(b.label));
      });
  }

  getSearchedCities() {
    const filteredCities = this.selectedCities.filter(item => item.toLowerCase().includes(this.searchText.toLowerCase()));
    return filteredCities.length ? filteredCities : undefined;
  }

  deleteCity(city: string) {
    this.selectedCities = this.selectedCities.filter(item => item != city);
  }

  close(isSave: boolean = false): void {
    this.activeModal.close({ isSave });
  }

  save(isAdd: boolean) {
    const textData = this.selectedCities.join(',');
    this.activeModal.close({ popupData: textData, isAdd, isSave: true });
  }
}
