import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { AutoFocusDirective } from "./auto-focus.directive";
import { BusyIfDirective } from "./busy-if.directive";
import { ButtonBusyDirective } from "./button-busy.directive";
import { FileDownloadService } from "./file-download.service";
import { FriendProfilePictureComponent } from "./friend-profile-picture.component";
import { LocalStorageService } from "./local-storage.service";
import { MomentFormatPipe } from "./moment-format.pipe";
import { MomentFromNowPipe } from "./moment-from-now.pipe";
import { ValidationMessagesComponent } from "./validation-messages.component";
import { EqualValidator } from "./validation/equal-validator.directive";
import { PasswordComplexityValidator } from "./validation/password-complexity-validator.directive";
import { MaxNumberValidatorDirective } from "./validation/maxNumberValidator.directive";
import { RequiredFieldValidator } from "./validation/requiredFieldValidator.directive";
import { NumberRangeValidator } from "./validation/numberRangeValidator.directive";
import { MaxLengthValidatorDirective } from "./validation/maxLengthValidator.directive";
import { MultipleEmailValidator } from "./validation/multipleEmailValidator.directive";
import { CampaignIdValidatorDirective } from "./validation/campaignIdValidator.directive";
import { SegmentRequiredQuantityValidator } from "./validation/segmentRequiredQuantityValidator.directive";
import { NullDefaultValueDirective } from "./null-value.directive";
import { ScriptLoaderService } from "./script-loader.service";
import { StyleLoaderService } from "./style-loader.service";
import { ArrayToTreeConverterService } from "./array-to-tree-converter.service";
import { TreeDataHelperService } from "./tree-data-helper.service";
import { LocalizePipe } from "@shared/common/pipes/localize.pipe";
import { PermissionPipe } from "@shared/common/pipes/permission.pipe";
import { FeatureCheckerPipe } from "@shared/common/pipes/feature-checker.pipe";
import { DatePickerMomentModifierDirective } from "./date-picker-moment-modifier.directive";
import { DateRangePickerMomentModifierDirective } from "./date-range-picker-moment-modifier.directive";
import { DigitOnlyDirective } from "./digit-only.directive";
import { statusTransformPipe } from "./status.pipe";
import { ExportWidthFieldValidator } from "./validation/exportWidthValidator.directive";
import { NoSpaceValidator } from "./validation/noSpaceValidator.directive";
import { MinNumberValidator } from "./validation/minNumberValidator.directive";
import { SingleLineConverterDirective } from "./single-line-converter.directive";

@NgModule({
  imports: [CommonModule],
  providers: [
    FileDownloadService,
    LocalStorageService,
    ScriptLoaderService,
    StyleLoaderService,
    ArrayToTreeConverterService,
    TreeDataHelperService
  ],
  declarations: [
    EqualValidator,
    PasswordComplexityValidator,
    MaxNumberValidatorDirective,
    MinNumberValidator,
    RequiredFieldValidator,
    NoSpaceValidator,
    NumberRangeValidator,
    ExportWidthFieldValidator,
    MultipleEmailValidator,
    CampaignIdValidatorDirective,
    SegmentRequiredQuantityValidator,
    MaxLengthValidatorDirective,
    ButtonBusyDirective,
    AutoFocusDirective,
    BusyIfDirective,
    FriendProfilePictureComponent,
    MomentFormatPipe,
    MomentFromNowPipe,
    ValidationMessagesComponent,
    NullDefaultValueDirective,
    DigitOnlyDirective,
    LocalizePipe,
    PermissionPipe,
    FeatureCheckerPipe,
    DatePickerMomentModifierDirective,
    DateRangePickerMomentModifierDirective,
    statusTransformPipe,
    SingleLineConverterDirective
  ],
  exports: [
    EqualValidator,
    PasswordComplexityValidator,
    MaxNumberValidatorDirective,
    MinNumberValidator,
    RequiredFieldValidator,
    NoSpaceValidator,
    NumberRangeValidator,
    ExportWidthFieldValidator,
    MultipleEmailValidator,

    CampaignIdValidatorDirective,
    SegmentRequiredQuantityValidator,
    MaxLengthValidatorDirective,
    ButtonBusyDirective,
    AutoFocusDirective,
    BusyIfDirective,
    FriendProfilePictureComponent,
    MomentFormatPipe,
    MomentFromNowPipe,
    ValidationMessagesComponent,
    NullDefaultValueDirective,
    DigitOnlyDirective,
    LocalizePipe,
    PermissionPipe,
    FeatureCheckerPipe,
    DatePickerMomentModifierDirective,
    DateRangePickerMomentModifierDirective,
    statusTransformPipe,
    SingleLineConverterDirective
  ]
})
export class UtilsModule {}
