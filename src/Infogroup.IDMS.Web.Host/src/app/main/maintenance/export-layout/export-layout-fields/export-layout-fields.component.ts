import { Component, OnInit, Injector, Input } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ExportLayoutsServiceProxy } from '@shared/service-proxies/service-proxies';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-export-layout-fields',
  templateUrl: './export-layout-fields.component.html',
  styleUrls: ['./export-layout-fields.component.css'],
  animations: [appModuleAnimation()]
})
export class ExportLayoutFieldsComponent extends AppComponentBase implements OnInit {

  @Input() ExportLayoutID: any;

  constructor(
    injector: Injector,
    private _exportLayoutServiceProxy: ExportLayoutsServiceProxy,
    private modalService: NgbModal
  ) {
    super(injector);
  }

  ngOnInit() {

    this._exportLayoutServiceProxy.getExportLayoutFields(this.ExportLayoutID).subscribe(
      result => {
          this.primengTableHelper.records = result;
          this.primengTableHelper.totalRecordsCount = result.length;
      }
  );
  }

}
