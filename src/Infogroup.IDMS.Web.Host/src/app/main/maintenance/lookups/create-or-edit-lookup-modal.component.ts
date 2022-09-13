import { Component, Injector, Input } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LookupsServiceProxy, CreateOrEditLookupDto } from '@shared/service-proxies/service-proxies';
import { id } from '@swimlane/ngx-charts/release/utils';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-create-or-edit-lookup-modal',
  templateUrl: './create-or-edit-lookup-modal.component.html',
})
export class CreateOrEditLookupModalComponent extends AppComponentBase {

  @Input() ID: number;
  lookupValueList: any = [];
  lookupValues: any = [];
  saving = false;
  lookup: CreateOrEditLookupDto = new CreateOrEditLookupDto();

  constructor(
    injector: Injector,
    private _lookupsServiceProxy: LookupsServiceProxy,
    public activeModal: NgbActiveModal
  ) {
    super(injector);
  }
  ngOnInit() {
    this.show(this.ID);
  }

  show(Id?: number): void {
    this._lookupsServiceProxy.getAllLookupsForDropdown().subscribe(result => {
      this.lookupValueList = result;
    });
    if (!Id) {
      this.lookup = new CreateOrEditLookupDto();
      this.lookup.iIsActive = true;
    } else {
      this._lookupsServiceProxy.getLookupForEdit(Id).subscribe(result => {
        this.lookup = result;
      });
    }
  }
  save(lookupForm: NgForm): void {
    if (lookupForm.dirty) {
      this.saving = true;
      this.lookup.id = this.ID;
      this._lookupsServiceProxy.createOrEdit(this.lookup)
        .pipe(finalize(() => { this.saving = false; }))
        .subscribe(() => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.activeModal.close({ saving: this.saving });
        });
    }
    else {
      this.notify.info(this.l('SavedSuccessfully'));
      this.activeModal.close({ saving: this.saving });
    }
  }
  close(): void {
    this.activeModal.close({ saving: this.saving });
  }
}
