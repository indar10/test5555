import { Component, Injector,Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { ListAutomatesServiceProxy,CreateOrEditIListAutomateDto} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-or-edit-listautomate-modal',
  templateUrl: './create-or-edit-listautomate-modal.component.html'
})
export class CreateOrEditListautomateModalComponent extends AppComponentBase {
  
  @Input() id: number;   
    active = false;
    saving = false;
    ftpServerInput : boolean=true;
    userIdInput: boolean = true;
    passwordInput: boolean = true;
    frequency:any = []; 
    listAutomateDto: CreateOrEditIListAutomateDto = new CreateOrEditIListAutomateDto();
    currentDate: Date = new Date();
    selectedFrequency:string="";
    validInterval:number=1;
    isIntervalDisabled:boolean=true;
    

  constructor(injector: Injector,public activeModal: NgbActiveModal,private _listAutomatesServiceProxy: ListAutomatesServiceProxy) { super(injector);}

  ngOnInit() {
    this.show(this.id);
  }

  show(id?: number): void {
    this._listAutomatesServiceProxy.getFrequency().subscribe(
      (result) => {
          this.frequency = result;          
      }
  );
    if (!id){
        this.listAutomateDto = new CreateOrEditIListAutomateDto();        
        this.listAutomateDto.iIsActive = true;       
        this.active = true;   
        
        this._listAutomatesServiceProxy.getServerDateForTime() 
        this._listAutomatesServiceProxy.getServerDateForTime() .subscribe(result => {
          this.currentDate = new Date(result.date);//.format("YYYY-MM-DD HH:mm"));         
          this.currentDate.setHours(1);
          this.currentDate.setMinutes(0);
          this.listAutomateDto.cScheduleTime = this.currentDate.toLocaleTimeString('en-US', { hour12: false, hour: '2-digit', minute: '2-digit' });
          this.listAutomateDto.buildId = 0;          
      })                        
    } else {
        this._listAutomatesServiceProxy.getIListAutomateForEdit(id).subscribe(result => {
            this.listAutomateDto = result;
            this.active = true;
            if(this.listAutomateDto.lK_ListConversionFrequency == 'D')
            {
              this.isIntervalDisabled = true;
            }
            else{
            this.isIntervalDisabled = false;}
        });
    }
}

save(listAutomateForm: NgForm): void {
  if (listAutomateForm.dirty) {
      this.saving = true;
      this.listAutomateDto.id = this.id;
       this._listAutomatesServiceProxy.createOrEdit(this.listAutomateDto)
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
    this.active = false;
    this.activeModal.close({ saving: this.saving });       
}

getFrequency()
{
  this._listAutomatesServiceProxy.getFrequency().
  subscribe(result => {    
    this.frequency=result
  });
}

getValidInterval(selectedFrequency)
  {    
    if(selectedFrequency == '')
    {
      this.isIntervalDisabled=true; 
    }
     if(selectedFrequency=='D'){
      this.isIntervalDisabled=true;  
      this.listAutomateDto.iInterval=0;    
    }
    else{
      this.isIntervalDisabled=false;  
      this.listAutomateDto.iInterval=1;
    }
       
  }
}
