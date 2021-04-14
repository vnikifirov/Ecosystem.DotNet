import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'titleCase'
})
export class TitleCasePipe implements PipeTransform {
  transform(input:string): string{
      // ? Converting text to TitleCase using Angular2 Pipes
      // ? URL : https://medium.com/@mwhitt.w/converting-text-to-titlecase-using-angular2-pipes-552b3bfa8e22
      if (!input) {
          return '';
      } else {
          return input.replace(/\w\S*/g, (txt => txt[0].toUpperCase() + txt.substr(1).toLowerCase() ));
      }
  }
}
