import { Directive, ElementRef, HostListener } from "@angular/core";

@Directive({
  selector: "[singleLineConverter]"
})
export class SingleLineConverterDirective {
  constructor(private _el: ElementRef) { }

  @HostListener("paste", ["$event"])
  convertToSingleLine(event: ClipboardEvent) {
    event.preventDefault();
    let inputText: string = event.clipboardData.getData("text/plain");
    if (typeof inputText !== "undefined" && inputText.length > 0) {
      let arrayOfLines = inputText.trim().match(/[^\r\n]+/g);
      if (arrayOfLines != null) {
        let outputText = arrayOfLines.join(", ");
        // 1. Remove spaces
        outputText = outputText.replace(/\s/g, '');
        // 2. Replace ",," spaces by ","
        outputText = outputText.replace(/,+/g, ',');
        // 3. Remove trailing comma(,)
        outputText = outputText.replace(/,\s*$/, '');
        document.execCommand("insertText", false, outputText);
      }
    }
  }
}
