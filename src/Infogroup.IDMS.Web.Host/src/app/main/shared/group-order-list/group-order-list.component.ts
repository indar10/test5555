import { AfterContentInit, Component, ContentChildren, EventEmitter, Input, Output, QueryList, TemplateRef } from '@angular/core';
import { ControlValueAccessor } from '@angular/forms';
import { PrimeTemplate } from 'primeng/components/common/shared';
import { FilterService } from './FilterService';
import { ObjectUtils } from './ObjectUtils';

@Component({
  selector: 'app-group-order-list',
  templateUrl: './group-order-list.component.html',
  styleUrls: ['./group-order-list.component.css']
})
export class GroupOrderListComponent implements AfterContentInit, ControlValueAccessor {
  @Input() style: any;
  @Input() styleClass: string;
  @Input() listStyle: any;
  @Input() listStyleClass: string;
  @Input() filter: boolean = false;
  @Input() disabled: boolean;
  @Input() filterPlaceHolder: string;
  @Input() ariaFilterLabel: string;
  @Input() filterMatchMode: string = 'contains';
  @Input() filterLocale: string;
  @Input() optionLabel: string;
  @Input() optionValue: string;
  @Input() group: boolean;
  @Input() optionGroupChildren: string = "items";
  @Input() optionGroupLabel: string;
  @Input() optionDisabled: string;
  @Input() readonly: boolean;
  @Input() dataKey: string;
  @Input() metaKeySelection: boolean = true;
  @Output() onChange: EventEmitter<any> = new EventEmitter();
  @Output() onClick: EventEmitter<any> = new EventEmitter();

  @ContentChildren(PrimeTemplate) templates: QueryList<any>;

  public itemTemplate: TemplateRef<any>;
  public groupTemplate: TemplateRef<any>;
  public headerTemplate: TemplateRef<any>;
  public footerTemplate: TemplateRef<any>;
  public emptyFilterTemplate: TemplateRef<any>;
  public emptyTemplate: TemplateRef<any>;

  public _filteredOptions: any[];
  public value: any;
  public optionTouched: boolean;
  public onModelChange: Function = () => { };
  public onModelTouched: Function = () => { };

  public _options: any[];
  @Input() get options(): any[] {
    return this._options;
  }
  set options(val: any[]) {
    this._options = val;

    if (this.hasFilter())
      this.activateFilter();
  }

  public _filterValue: string;
  @Input() get filterValue(): string {
    return this._filterValue;
  }
  set filterValue(val: string) {
    this._filterValue = val;
    this.activateFilter();
  }

  constructor(private filterService: FilterService) {
    this.filterService = new FilterService();
  }

  ngAfterContentInit() {
    this.templates.forEach((item) => {
      switch (item.getType()) {
        case 'item':
          this.itemTemplate = item.template;
          break;

        case 'group':
          this.groupTemplate = item.template;
          break;

        case 'header':
          this.headerTemplate = item.template;
          break;

        case 'footer':
          this.footerTemplate = item.template;
          break;

        case 'empty':
          this.emptyTemplate = item.template;
          break;

        case 'emptyfilter':
          this.emptyFilterTemplate = item.template;
          break;

        default:
          this.itemTemplate = item.template;
          break;
      }
    });
  }

  writeValue(value: any): void {
    this.value = value;
  }

  registerOnChange(fn: Function): void {
    this.onModelChange = fn;
  }

  registerOnTouched(fn: Function): void {
    this.onModelTouched = fn;
  }

  get optionsToRender(): any[] {
    return this._filteredOptions || this.options;
  }

  get emptyMessageLabel(): string {
    return "No results found";
  }

  isOptionDisabled(option: any) {
    return this.optionDisabled ? ObjectUtils.resolveFieldData(option, this.optionDisabled) : (option.disabled !== undefined ? option.disabled : false);
  }

  getOptionValue(option: any) {
    return this.optionValue ? ObjectUtils.resolveFieldData(option, this.optionValue) : (this.optionLabel || option.value === undefined ? option : option.value);
  }

  isSelected(option: any) {
    let selected = false;
    let optionValue = this.getOptionValue(option);
    selected = ObjectUtils.equals(this.value, optionValue, this.dataKey);
    return selected;
  }

  onOptionClickSingle(event, option) {
    let selected = this.isSelected(option);
    let valueChanged = false;
    let metaSelection = this.optionTouched ? false : this.metaKeySelection;

    if (metaSelection) {
      let metaKey = (event.metaKey || event.ctrlKey);

      if (selected) {
        if (metaKey) {
          this.value = null;
          valueChanged = true;
        }
      }
      else {
        this.value = this.getOptionValue(option);
        valueChanged = true;
      }
    }
    else {
      this.value = selected ? null : this.getOptionValue(option);
      valueChanged = true;
    }

    if (valueChanged) {
      this.onModelChange(this.value);
      this.onChange.emit({
        originalEvent: event,
        value: this.value
      });
    }
  }

  onOptionClick(event: Event, option: any) {
    if (this.disabled || this.isOptionDisabled(option) || this.readonly) {
      return;
    }
    this.onOptionClickSingle(event, option);
    this.onClick.emit({
      originalEvent: event,
      option: option,
      value: this.value
    });
    this.optionTouched = false;
    this._filterValue = "";
    this.activateFilter();
  }

  onFilter(event: KeyboardEvent) {
    this._filterValue = (<HTMLInputElement>event.target).value;
    this.activateFilter();
  }

  isEmpty(optionsToDisplay) {
    return !optionsToDisplay || (optionsToDisplay && optionsToDisplay.length === 0);
  }

  hasFilter() {
    return this._filterValue && this._filterValue.trim().length > 0;
  }

  getOptionGroupLabel(optionGroup: any) {
    return this.optionGroupLabel ? ObjectUtils.resolveFieldData(optionGroup, this.optionGroupLabel) : (optionGroup.label != undefined ? optionGroup.label : optionGroup);
  }

  scrollToView(optionGroup: any) {
    this._filterValue = "";
    this.activateFilter();
    const timeout = setTimeout(() => {
      const fieldID: string = this.optionGroupLabel ? ObjectUtils.resolveFieldData(optionGroup, this.optionGroupLabel) : (optionGroup.label != undefined ? optionGroup.label : optionGroup);
      const id = fieldID.replace(/\s/g, '');
      document.getElementById(id).scrollIntoView({
        behavior: "smooth"
      });
      clearTimeout(timeout);
    }, 500)
  }

  getOptionGroupLabelForId(optionGroup: any) {
    const fieldID: string = this.optionGroupLabel ? ObjectUtils.resolveFieldData(optionGroup, this.optionGroupLabel) : (optionGroup.label != undefined ? optionGroup.label : optionGroup);
    return fieldID.replace(/\s/g, '');
  }

  getOptionLabel(option: any) {
    return this.optionLabel ? ObjectUtils.resolveFieldData(option, this.optionLabel) : (option.label != undefined ? option.label : option);
  }

  getOptionGroupChildren(optionGroup: any) {
    return this.optionGroupChildren ? ObjectUtils.resolveFieldData(optionGroup, this.optionGroupChildren) : optionGroup.items;
  }

  activateFilter() {
    if (this.hasFilter() && this._options) {
      if (this.group) {
        let searchFields: string[] = (this.optionLabel || 'label').split(',');

        let filteredGroups = [];
        for (let optgroup of this.options) {
          let filteredSubOptions = this.filterService.filter(this.getOptionGroupChildren(optgroup), searchFields, this.filterValue, this.filterMatchMode, this.filterLocale);
          if (filteredSubOptions && filteredSubOptions.length) {
            filteredGroups.push({ ...optgroup, ...{ [this.optionGroupChildren]: filteredSubOptions } });
          }
        }

        this._filteredOptions = filteredGroups;
      }
      else {
        this._filteredOptions = this._options.filter(option => this.filterService.filters[this.filterMatchMode](this.getOptionLabel(option), this._filterValue, this.filterLocale));
      }
    }
    else {
      this._filteredOptions = null;
    }
  }
}
