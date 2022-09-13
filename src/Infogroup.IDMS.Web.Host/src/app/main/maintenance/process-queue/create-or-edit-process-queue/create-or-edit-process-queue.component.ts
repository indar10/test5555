import { Component, Injector, Input, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditProcessQueueDto, ProcessQueuesServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-create-or-edit-process-queue',
  templateUrl: './create-or-edit-process-queue.component.html',
  styleUrls: ['./create-or-edit-process-queue.component.css']
})
export class CreateOrEditProcessQueueComponent extends AppComponentBase {
  saving:boolean= false;
  queueType:any=[];
  processType:any=[];
  @Input() id:number;
  processQueue:CreateOrEditProcessQueueDto= new CreateOrEditProcessQueueDto();
  showEmptyProcessType: boolean=false;
  constructor(private injector: Injector,
    public activeModal: NgbActiveModal,private processQueueServiceProxy: ProcessQueuesServiceProxy) { super(injector)}

  ngOnInit() {
    this.fillDropDown();
    if(this.id){
      this.show(this.id);
    }
  }
  show(id){
    this.processQueueServiceProxy.getProcessQueueForView(id).subscribe(data=>{

      this.processQueue.cQueueName=data.processQueue.cQueueName
      this.processQueue.cDescription=data.processQueue.cDescription
      this.processQueue.lK_QueueType=data.processQueue.lK_QueueType
      if(this.processQueue.lK_QueueType){
        this.FillProcessType();
      }
      this.processQueue.lK_ProcessType=data.processQueue.lK_ProcessType
      this.processQueue.iAllowedThreadCount=data.processQueue.iAllowedThreadCount
      this.processQueue.cCreatedBy=data.processQueue.cCreatedBy
      this.processQueue.dCreatedDate=data.processQueue.dCreatedDate
    })
  }
  fillDropDown(){
    this.processQueueServiceProxy.getAllLookupsForDropdown().subscribe(c=>{
      this.queueType=c
    })
  }
  FillProcessType(form?:NgForm){
    if(this.processQueue.lK_QueueType){
      this.processQueueServiceProxy.getAllLookupsOfProcessTypeForDropdown(this.processQueue.lK_QueueType).subscribe(data=>{
        this.processType=data;
       if(data.length){
        this.processType=data;
        this.processQueue.lK_ProcessType= this.processType[0].value
       }else{
        this.processType.push({'value':'','label':"No data Available "});
        this.processQueue.lK_ProcessType=this.processType[0].value;
       }
      })
      if(form && form.controls){
        form.controls.processType.reset();
      }
     
    }
   
  }
  save(Form?:NgForm){
    if(!this.processQueue.cDescription){
      this.processQueue.cDescription='';
    }
    if(Form.dirty){
      this.saving=true;
      if(this.id){
        this.processQueue.id=this.id
      }
    this.processQueueServiceProxy.createOrEdit(this.processQueue).pipe(finalize(() => { this.saving = false; })).
    subscribe(()=>{
      this.notify.info(this.l('SavedSuccessfully'));
      this.activeModal.close({ saving: this.saving });
    });
    }
   else{
    this.notify.info(this.l('SavedSuccessfully'));
    this.activeModal.close({ saving: this.saving });
   }
  }
  close(){
    this.activeModal.close({ saving: this.saving });
  }
}

