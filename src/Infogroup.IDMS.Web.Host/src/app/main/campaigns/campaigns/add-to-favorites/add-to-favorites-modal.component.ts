import { Component, OnInit, Input, Injector } from "@angular/core";
import {
  UserSavedSelectionsServiceProxy,
  CreateOrEditUserSavedSelectionDto
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { finalize } from "rxjs/operators";

@Component({
  selector: "add-to-favorites",
  templateUrl: "./add-to-favorites-modal.component.html",
  styleUrls: ["./add-to-favorites-modal.component.css"]
})
export class AddToFavoritesComponent extends AppComponentBase
  implements OnInit {
  @Input() sourceSegmentId: number;
  @Input() campaignId: number;
  @Input() databaseId: number;
  input: CreateOrEditUserSavedSelectionDto;
  saving: boolean = false;
  constructor(
    injector: Injector,
    private activeModal: NgbActiveModal,
    private _userSavedSelectionsServiceProxy: UserSavedSelectionsServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.input = CreateOrEditUserSavedSelectionDto.fromJS({
      cDescription: "",
      iIsActive: true,
      cChannelType: "P",
      iIsDefault: false,
      databaseId: this.databaseId,
      id: 0
    });
  }

  save(): void {
    this._userSavedSelectionsServiceProxy
      .create(this.sourceSegmentId, this.input)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(result => {
        if (result.toUpperCase() === "UNDER CONSTRUCTION") {
          this.message.info(result);
        } else {
          this.notify.success(result);
          this.activeModal.close();
        }
      });
  }

  close(): void {
    this.activeModal.close();
  }
}
