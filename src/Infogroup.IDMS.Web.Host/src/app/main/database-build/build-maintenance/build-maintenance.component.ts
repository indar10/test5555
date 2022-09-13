import { Component, OnInit,Injector } from '@angular/core';
import { AppComponentBase } from "@shared/common/app-component-base";

@Component({
  selector: 'app-build-maintenance',
  templateUrl: './build-maintenance.component.html',
  styleUrls: ['./build-maintenance.component.css']
})
export class BuildMaintenanceComponent extends AppComponentBase{

  constructor( injector: Injector) {
    super(injector);
   }
  ngOnInit() {
  }

}
